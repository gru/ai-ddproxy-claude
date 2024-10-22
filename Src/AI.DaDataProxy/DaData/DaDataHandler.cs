using System.Text.Json;
using Microsoft.Extensions.Options;
using AI.DaDataProxy.Validators;
using RestEase;

namespace AI.DaDataProxy.DaData;

/// <summary>
/// Обработчик запросов к DaData API с поддержкой кэширования и ограничения количества запросов.
/// </summary>
public class DaDataHandler
{
    private readonly IDaDataApi _daDataApi;
    private readonly DaDataCachingOptions _cachingOptions;
    private readonly IRedisCache _redisCache;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="DaDataHandler"/>.
    /// </summary>
    /// <param name="daDataApi">Интерфейс для взаимодействия с DaData API.</param>
    /// <param name="cachingOptions">Настройки кэширования.</param>
    /// <param name="redisCache">Кэш Redis.</param>
    public DaDataHandler(
        IDaDataApi daDataApi, 
        IOptions<DaDataCachingOptions> cachingOptions,
        IRedisCache redisCache)
    {
        _daDataApi = daDataApi;
        _cachingOptions = cachingOptions.Value;
        _redisCache = redisCache;
    }

    /// <summary>
    /// Обрабатывает запрос к DaData API, используя кэширование и проверку лимитов.
    /// </summary>
    /// <param name="path">Путь запроса к API.</param>
    /// <param name="body">Тело запроса в формате JSON.</param>
    /// <returns>Результат запроса в виде строки JSON.</returns>
    /// <exception cref="Exception">Возникает, если превышен дневной лимит запросов.</exception>
    public async Task<string> HandleRequestAsync(string path, string body)
    {
        var currentCount = 0L;
        var utcNow = DateTime.UtcNow;
        
        if (_cachingOptions.DailyRequestLimit > 0)
        {
            currentCount = await _redisCache.GetDailyRequestCountAsync(utcNow);
            if (currentCount >= _cachingOptions.DailyRequestLimit)
            {
                   throw new DaDataTooManyRequestsException("Daily request limit exceeded");
            }
        }

        var cacheKey = CacheKeys.RequestCache(path, body);
        var cachedResult = await _redisCache.GetCachedQueryAsync(cacheKey);
        if (!string.IsNullOrEmpty(cachedResult))
        {
            return cachedResult;
        }

        try
        {
            var result = await _daDataApi.ProxyRequestAsync(path, body);

            if (!string.IsNullOrEmpty(result))
            {
                if (_cachingOptions.DailyRequestLimit > 0)
                {
                    await _redisCache.IncrementDailyRequestCounter(utcNow, currentCount);
                }

                var cacheExpiration = GetCacheExpirationForRequest(path, result);
                await _redisCache.SetCachedQueryAsync(cacheKey, result, cacheExpiration);

                if (IsLegalEntityByInnRequest(path))
                {
                    var legalEntityCacheExpiration = TimeSpan.FromDays(_cachingOptions.LegalEntityCacheDurationInDays);
                    await CreateAdditionalInnCache(body, result, legalEntityCacheExpiration);
                }
            }

            return result;
        }
        catch (ApiException ex)
        {
            throw new DaDataIntegrationException("Ошибка при обращении к сервису DaData", ex);
        }
    }

    private TimeSpan GetCacheExpirationForRequest(string path, string result)
    {
        if (IsLegalEntityByInnRequest(path))
        {
            var jsonResult = JsonSerializer.Deserialize<JsonElement>(result);
            if (jsonResult.TryGetProperty("suggestions", out var suggestions) && suggestions.GetArrayLength() > 0)
            {
                return TimeSpan.FromDays(_cachingOptions.LegalEntityCacheDurationInDays);
            }
            else
            {
                return TimeSpan.FromDays(_cachingOptions.EmptyLegalEntityCacheDurationInDays);
            }
        }
        else if (IsAddressRequest(path))
        {
            return TimeSpan.FromDays(_cachingOptions.AddressCacheDurationInDays);
        }
        else
        {
            return TimeSpan.FromHours(_cachingOptions.DefaultCacheDurationInHours);
        }
    }

    private async Task CreateAdditionalInnCache(string body, string result, TimeSpan expiration)
    {
        var jsonBody = JsonSerializer.Deserialize<JsonElement>(body);
        if (jsonBody.TryGetProperty("query", out var queryElement))
        {
            var inn = queryElement.GetString();
            if (InnValidator.IsValid(inn))
            {
                var innCacheKey = CacheKeys.InnQueryCache(inn!);
                await _redisCache.SetCachedQueryAsync(innCacheKey, result, expiration);
            }
        }
    }

    private static bool IsLegalEntityByInnRequest(string path) => path.Contains("findById/party");
    
    private static bool IsAddressRequest(string path) => path.Contains("address");
}
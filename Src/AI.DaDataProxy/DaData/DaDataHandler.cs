using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using AI.DaDataProxy.Validators;

namespace AI.DaDataProxy.DaData;

/// <summary>
/// Обработчик запросов к сервису DaData с поддержкой кеширования и специальной обработкой ИНН.
/// </summary>
public class DaDataHandler
{
    private readonly IDaDataApi _daDataApi;
    private readonly IDistributedCache _cache;
    private readonly DaDataOptions _options;
    private readonly DaDataCachingOptions _cachingOptions;

    public DaDataHandler(
        IDaDataApi daDataApi, 
        IDistributedCache cache, 
        IOptions<DaDataOptions> options,
        IOptions<DaDataCachingOptions> cachingOptions)
    {
        _daDataApi = daDataApi;
        _cache = cache;
        _options = options.Value;
        _cachingOptions = cachingOptions.Value;
    }

    public async Task<string> HandleRequestAsync(string path, string body)
    {
        var cacheKey = GenerateCacheKey(path, body);
        var cachedResult = await _cache.GetStringAsync(cacheKey);

        if (cachedResult != null)
        {
            return cachedResult;
        }

        var result = await _daDataApi.ProxyRequestAsync(path, body);

        if (!string.IsNullOrEmpty(result))
        {
            var cacheExpiration = GetCacheExpirationForRequest(path, body, result);
            var cacheEntryOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = cacheExpiration
            };

            await _cache.SetStringAsync(cacheKey, result, cacheEntryOptions);

            // Специальная обработка для запросов по ИНН
            if (IsLegalEntityByInnRequest(path))
            {
                await CreateAdditionalInnCache(body, result, cacheExpiration);
            }
        }

        return result;
    }

    private TimeSpan GetCacheExpirationForRequest(string path, string body, string result)
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
        else if (path.Contains("address"))
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
                var innCacheKey = $"inn:{inn}";
                await _cache.SetStringAsync(innCacheKey, result, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiration
                });
            }
        }
    }

    private static string GenerateCacheKey(string path, string body)
    {
        using var sha256 = SHA256.Create();
        var inputBytes = Encoding.UTF8.GetBytes($"{path}:{body}");
        var hashBytes = sha256.ComputeHash(inputBytes);
        return Convert.ToBase64String(hashBytes);
    }

    /// <summary>
    /// Проверяет, является ли запрос запросом на поиск юридического лица по ИНН.
    /// </summary>
    /// <param name="path">Путь запроса.</param>
    /// <returns>true, если запрос на поиск юридического лица по ИНН; иначе false.</returns>
    private static bool IsLegalEntityByInnRequest(string path)
    {
        return path.Contains("findById/party");
    }
}
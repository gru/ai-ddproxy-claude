using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using AI.DaDataProxy.Validators;
using StackExchange.Redis;

namespace AI.DaDataProxy.DaData;

public class DaDataHandler
{
    private readonly IDaDataApi _daDataApi;
    private readonly DaDataOptions _options;
    private readonly DaDataCachingOptions _cachingOptions;
    private readonly IConnectionMultiplexer _redisConnection;

    public DaDataHandler(
        IDaDataApi daDataApi, 
        IOptions<DaDataOptions> options,
        IOptions<DaDataCachingOptions> cachingOptions,
        IConnectionMultiplexer redisConnection)
    {
        _daDataApi = daDataApi;
        _options = options.Value;
        _cachingOptions = cachingOptions.Value;
        _redisConnection = redisConnection;
    }

    public async Task<string> HandleRequestAsync(string path, string body)
    {
        var counterKey = $"dadata:daily_request_counter:{DateTime.UtcNow:yyyyMMdd}";
        long currentCount = 0;

        if (_cachingOptions.DailyRequestLimit > 0)
        {
            currentCount = await GetDailyRequestCount(counterKey);
            if (currentCount >= _cachingOptions.DailyRequestLimit)
            {
                throw new Exception("Daily request limit exceeded");
            }
        }

        var cacheKey = GenerateCacheKey(path, body);
        var db = _redisConnection.GetDatabase();
        var cachedResult = await db.StringGetAsync(cacheKey);

        if (!cachedResult.IsNullOrEmpty)
        {
            return cachedResult!;
        }

        var result = await _daDataApi.ProxyRequestAsync(path, body);

        if (!string.IsNullOrEmpty(result))
        {
            // Инкрементируем счетчик только после успешного запроса
            if (_cachingOptions.DailyRequestLimit > 0)
            {
                await IncrementDailyRequestCounter(counterKey, currentCount);
            }

            var cacheExpiration = GetCacheExpirationForRequest(path, body, result);
            await db.StringSetAsync(cacheKey, result, cacheExpiration);

            if (IsLegalEntityByInnRequest(path))
            {
                await CreateAdditionalInnCache(body, result, cacheExpiration);
            }
        }

        return result;
    }

    private async Task<long> GetDailyRequestCount(string counterKey)
    {
        var db = _redisConnection.GetDatabase();
        var currentCount = await db.StringGetAsync(counterKey);
        return currentCount.HasValue ? (long)currentCount : 0;
    }

    private async Task IncrementDailyRequestCounter(string counterKey, long currentCount)
    {
        var db = _redisConnection.GetDatabase();

        var transaction = db.CreateTransaction();
        var incrementTask = transaction.StringIncrementAsync(counterKey);

        // Устанавливаем время жизни ключа только если это новый ключ (текущее значение 0)
        if (currentCount == 0)
        {
            await transaction.KeyExpireAsync(counterKey, TimeSpan.FromHours(_cachingOptions.RequestCounterExpirationHours));
        }
        
        if (!await transaction.ExecuteAsync())
        {
            throw new Exception("Failed to increment request counter");
        }

        await incrementTask;
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
                var db = _redisConnection.GetDatabase();
                await db.StringSetAsync(innCacheKey, result, expiration);
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

    private static bool IsLegalEntityByInnRequest(string path)
    {
        return path.Contains("findById/party");
    }
}
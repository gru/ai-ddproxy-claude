using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace AI.DaDataProxy.DaData;

/// <summary>
/// Класс для работы с кэшем Redis.
/// </summary>
public class RedisCache : IRedisCache
{
    private readonly IConnectionMultiplexer _redisConnection;
    private readonly IOptions<DaDataCachingOptions> _cachingOptions;

    /// <summary>
    /// Инициализирует новый экземпляр класса RedisCache.
    /// </summary>
    /// <param name="redisConnection">Подключение к Redis.</param>
    /// <param name="cachingOptions">Настройки кэширования.</param>
    public RedisCache(IConnectionMultiplexer redisConnection, IOptions<DaDataCachingOptions> cachingOptions)
    {
        _redisConnection = redisConnection;
        _cachingOptions = cachingOptions;
    }

    /// <summary>
    /// Получает количество запросов, сделанных за день.
    /// </summary>
    /// <param name="dateTime">Дата, для которой нужно получить количество запросов.</param>
    /// <returns>Количество запросов за указанный день.</returns>
    public async Task<long> GetDailyRequestCountAsync(DateTime dateTime)
    {
        var counterKey = CacheKeys.DailyRequestCounter(dateTime);
        var db = _redisConnection.GetDatabase();
        var currentCount = await db.StringGetAsync(counterKey);
        return currentCount.HasValue ? (long)currentCount : 0;
    }

    /// <summary>
    /// Увеличивает счетчик ежедневных запросов.
    /// </summary>
    /// <param name="dateTime">Дата, для которой нужно увеличить счетчик.</param>
    /// <param name="currentCount">Текущее значение счетчика.</param>
    /// <returns>Задача, представляющая асинхронную операцию.</returns>
    public async Task IncrementDailyRequestCounter(DateTime dateTime, long currentCount)
    {
        var counterKey = CacheKeys.DailyRequestCounter(dateTime);
        var db = _redisConnection.GetDatabase();
        var transaction = db.CreateTransaction();
        
        if (currentCount == 0)
        {
            await transaction.KeyExpireAsync(counterKey, TimeSpan.FromHours(_cachingOptions.Value.RequestCounterExpirationHours));
        }
        
        await transaction.StringIncrementAsync(counterKey);
        
        if (!await transaction.ExecuteAsync())
        {
            throw new Exception("Failed to increment request counter");
        }
    }

    /// <summary>
    /// Получает кэшированный результат запроса по ключу.
    /// </summary>
    /// <param name="cacheKey">Ключ кэша.</param>
    /// <returns>Кэшированный результат запроса или null, если кэш не найден.</returns>
    public async Task<string?> GetCachedQueryAsync(string cacheKey)
    {
        var db = _redisConnection.GetDatabase();
        return await db.StringGetAsync(cacheKey);
    }

    /// <summary>
    /// Сохраняет результат запроса в кэш.
    /// </summary>
    /// <param name="cacheKey">Ключ кэша.</param>
    /// <param name="value">Значение для сохранения в кэш.</param>
    /// <param name="expiration">Время истечения кэша.</param>
    /// <returns>Задача, представляющая асинхронную операцию.</returns>
    public async Task SetCachedQueryAsync(string cacheKey, string value, TimeSpan expiration)
    {
        var db = _redisConnection.GetDatabase();
        await db.StringSetAsync(cacheKey, value, expiration);
    }
}
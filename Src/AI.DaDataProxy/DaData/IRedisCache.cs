namespace AI.DaDataProxy.DaData;

/// <summary>
/// Интерфейс для работы с кэшем Redis в контексте DaData.
/// </summary>
public interface IRedisCache
{
    /// <summary>
    /// Получает количество запросов, сделанных за день.
    /// </summary>
    /// <param name="dateTime">Дата, для которой нужно получить количество запросов.</param>
    /// <returns>Количество запросов за указанный день.</returns>
    Task<long> GetDailyRequestCountAsync(DateTime dateTime);

    /// <summary>
    /// Увеличивает счетчик ежедневных запросов.
    /// </summary>
    /// <param name="dateTime">Дата, для которой нужно увеличить счетчик.</param>
    /// <param name="currentCount">Текущее значение счетчика.</param>
    /// <returns>Задача, представляющая асинхронную операцию.</returns>
    Task IncrementDailyRequestCounter(DateTime dateTime, long currentCount);

    /// <summary>
    /// Получает кэшированный результат запроса по ключу.
    /// </summary>
    /// <param name="cacheKey">Ключ кэша.</param>
    /// <returns>Кэшированный результат запроса или null, если кэш не найден.</returns>
    Task<string?> GetCachedQueryAsync(string cacheKey);

    /// <summary>
    /// Сохраняет результат запроса в кэш.
    /// </summary>
    /// <param name="cacheKey">Ключ кэша.</param>
    /// <param name="value">Значение для сохранения в кэш.</param>
    /// <param name="expiration">Время истечения кэша.</param>
    /// <returns>Задача, представляющая асинхронную операцию.</returns>
    Task SetCachedQueryAsync(string cacheKey, string value, TimeSpan expiration);
}
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace AI.DaDataProxy.DaData;

/// <summary>
/// Обработчик запросов к сервису DaData с поддержкой кеширования.
/// </summary>
public class DaDataHandler
{
    private readonly IDaDataApi _daDataApi;
    private readonly IDistributedCache _cache;
    private readonly DaDataOptions _options;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="DaDataHandler"/>.
    /// </summary>
    /// <param name="daDataApi">Интерфейс для взаимодействия с API DaData.</param>
    /// <param name="cache">Распределенный кеш для хранения результатов запросов.</param>
    /// <param name="options">Настройки DaData.</param>
    public DaDataHandler(IDaDataApi daDataApi, IDistributedCache cache, IOptions<DaDataOptions> options)
    {
        _daDataApi = daDataApi;
        _cache = cache;
        _options = options.Value;
    }

    /// <summary>
    /// Обрабатывает запрос к DaData с поддержкой кеширования.
    /// </summary>
    /// <param name="path">Путь API запроса.</param>
    /// <param name="body">Тело запроса в формате JSON.</param>
    /// <param name="cacheExpiration">Необязательный параметр. Время жизни кеша для этого запроса.</param>
    /// <returns>Результат запроса в формате JSON.</returns>
    public async Task<string> HandleRequestAsync(string path, string body, TimeSpan? cacheExpiration = null)
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
            var cacheEntryOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = cacheExpiration ?? TimeSpan.FromMinutes(60)
            };

            await _cache.SetStringAsync(cacheKey, result, cacheEntryOptions);
        }

        return result;
    }

    /// <summary>
    /// Генерирует уникальный ключ кеша на основе пути и тела запроса.
    /// </summary>
    /// <param name="path">Путь API запроса.</param>
    /// <param name="body">Тело запроса в формате JSON.</param>
    /// <returns>Уникальный ключ кеша.</returns>
    private static string GenerateCacheKey(string path, string body)
    {
        using var sha256 = SHA256.Create();
        var inputBytes = Encoding.UTF8.GetBytes($"{path}:{body}");
        var hashBytes = sha256.ComputeHash(inputBytes);
        return Convert.ToBase64String(hashBytes);
    }
}
using RestEase;

namespace AI.DaDataProxy.DaData;

/// <summary>
/// Интерфейс для проксирования запросов к API DaData.
/// </summary>
[Header("Content-Type", "application/json")]
public interface IDaDataApi
{
    /// <summary>
    /// Проксирует запрос к API DaData.
    /// </summary>
    /// <param name="path">Часть пути API после "suggestions/api/4_1/rs/".</param>
    /// <param name="body">Тело запроса в формате JSON.</param>
    /// <returns>Ответ от API DaData в формате JSON.</returns>
    [Post("suggestions/api/4_1/rs/{path}")]
    Task<string> ProxyRequestAsync([Path(UrlEncode = false)] string path, [Body] string body);
}
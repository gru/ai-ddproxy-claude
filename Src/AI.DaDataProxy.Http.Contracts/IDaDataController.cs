using RestEase;

namespace AI.DaDataProxy.Http.Contracts;

/// <summary>
/// Интерфейс для контроллера DaData, определяющий методы для проксирования запросов к сервису DaData.
/// </summary>
public interface IDaDataController
{
    /// <summary>
    /// Проксирует запрос к API DaData.
    /// </summary>
    /// <param name="path">Часть пути API после "suggestions/api/4_1/rs/".</param>
    /// <param name="body">Тело запроса в формате JSON.</param>
    /// <returns>Ответ от API DaData в формате JSON.</returns>
    [Post("/suggestions/api/4_1/rs/{path}")]
    Task<string> ProxyRequest([Path(UrlEncode = false)] string path, [Body] HttpContent body);
}

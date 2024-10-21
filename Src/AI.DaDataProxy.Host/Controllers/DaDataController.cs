using System.Text.Json.Nodes;
using AI.DaDataProxy.DaData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace AI.DaDataProxy.Host.Controllers;

/// <summary>
/// Контроллер для проксирования запросов к сервису DaData.
/// </summary>
[ApiController]
[FeatureGate(FeatureToggles.ApiEnabled)]
public class DaDataController : ControllerBase
{
    private readonly DaDataHandler _handler;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="DaDataController"/>.
    /// </summary>
    /// <param name="handler">Обработчик запросов к DaData.</param>
    public DaDataController(DaDataHandler handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// Проксирует запрос к API DaData.
    /// </summary>
    /// <param name="path">Часть пути API после "suggestions/api/4_1/rs/".</param>
    /// <param name="body">Тело запроса в формате JSON.</param>
    /// <returns>Ответ от API DaData в формате JSON.</returns>
    [HttpPost("/suggestions/api/4_1/rs/{*path}")]
    public async Task<IActionResult> ProxyRequest([FromRoute] string path, [FromBody] JsonObject body)
    {
        var result = await _handler.HandleRequestAsync(path, body.ToJsonString());
        return Content(result, "application/json");
    }
}

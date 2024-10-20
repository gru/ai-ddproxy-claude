using System.ComponentModel.DataAnnotations;

namespace AI.DaDataProxy.DaData;

/// <summary>
/// Содержит настройки для подключения к сервису DaData.
/// </summary>
public class DaDataOptions
{
    /// <summary>
    /// API ключ для авторизации в сервисе DaData.
    /// Этот ключ необходим для выполнения запросов к API.
    /// </summary>
    [Required]
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Секретный ключ для дополнительной авторизации в сервисе DaData.
    /// Используется для некоторых специфических запросов, требующих повышенной безопасности.
    /// </summary>
    [Required]
    public string Secret { get; set; } = string.Empty;

    /// <summary>
    /// Базовый URL-адрес API сервиса DaData.
    /// Например, "https://suggestions.dadata.ru".
    /// </summary>
    [Required]
    public string BaseUrl { get; set; } = string.Empty;
}
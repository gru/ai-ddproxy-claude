using System.ComponentModel.DataAnnotations;

namespace AI.DaDataProxy.DaData;

/// <summary>
/// Настройки кеширования для различных типов запросов к сервису DaData.
/// Этот класс позволяет конфигурировать время жизни кеша для разных сценариев использования API.
/// </summary>
public class DaDataCachingOptions
{
    /// <summary>
    /// Время жизни кеша для запросов информации о юридических лицах по ИНН при наличии результатов.
    /// </summary>
    [Range(1, int.MaxValue)]
    public int LegalEntityCacheDurationInDays { get; set; } = 30;

    /// <summary>
    /// Время жизни кеша для запросов информации о юридических лицах по ИНН при отсутствии результатов.
    /// Используется для кеширования отрицательных результатов, например, для недавно зарегистрированных компаний.
    /// </summary>
    [Range(1, int.MaxValue)]
    public int EmptyLegalEntityCacheDurationInDays { get; set; } = 1;

    /// <summary>
    /// Время жизни кеша для запросов, связанных с адресами.
    /// </summary>
    [Range(1, int.MaxValue)]
    public int AddressCacheDurationInDays { get; set; } = 7;

    /// <summary>
    /// Время жизни кеша по умолчанию для всех остальных типов запросов.
    /// </summary>
    [Range(1, int.MaxValue)]
    public int DefaultCacheDurationInHours { get; set; } = 1;
}
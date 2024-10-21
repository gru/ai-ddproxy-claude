using System;

namespace AI.DaDataProxy.DaData;

/// <summary>
/// Представляет исключение, которое возникает при превышении лимита запросов к сервису DaData.
/// </summary>
public class DaDataTooManyRequests : Exception
{
    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="DaDataTooManyRequests"/> с указанным сообщением об ошибке.
    /// </summary>
    /// <param name="message">Сообщение об ошибке с объяснением причины исключения.</param>
    public DaDataTooManyRequests(string message) : base(message)
    {
    }

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="DaDataTooManyRequests"/> с указанным сообщением об ошибке
    /// и ссылкой на внутреннее исключение, которое является причиной данного исключения.
    /// </summary>
    /// <param name="message">Сообщение об ошибке �� объяснением причины исключения.</param>
    /// <param name="innerException">Исключение, вызвавшее текущее исключение, или null, если внутреннее исключение не задано.</param>
    public DaDataTooManyRequests(string message, Exception innerException) : base(message, innerException)
    {
    }
}
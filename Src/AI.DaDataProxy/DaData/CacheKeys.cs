using System.Security.Cryptography;
using System.Text;

namespace AI.DaDataProxy.DaData;

/// <summary>
/// Статический класс для генерации ключей кэша.
/// </summary>
public static class CacheKeys
{
    /// <summary>
    /// Генерирует ключ для ежедневного счетчика запросов.
    /// </summary>
    /// <param name="date">Дата, для которой генерируется ключ.</param>
    /// <returns>Строка ключа для ежедневного счетчика запросов.</returns>
    public static string DailyRequestCounter(DateTime date) =>
        $"dadata:daily_request_counter:{date:yyyyMMdd}";

    /// <summary>
    /// Генерирует ключ кэша для запроса.
    /// </summary>
    /// <param name="path">Путь запроса.</param>
    /// <param name="body">Тело запроса.</param>
    /// <returns>Строка ключа кэша для запроса.</returns>
    public static string RequestCache(string path, string body)
    {
        using var sha256 = SHA256.Create();
        var inputBytes = Encoding.UTF8.GetBytes($"{path}:{body}");
        var hashBytes = sha256.ComputeHash(inputBytes);
        return Convert.ToBase64String(hashBytes);
    }

    /// <summary>
    /// Генерирует ключ кэша для ИНН.
    /// </summary>
    /// <param name="inn">ИНН.</param>
    /// <returns>Строка ключа кэша для ИНН.</returns>
    public static string InnQueryCache(string inn) => $"inn:{inn}";
}
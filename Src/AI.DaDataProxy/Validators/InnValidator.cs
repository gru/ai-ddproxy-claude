using System.Text.RegularExpressions;

namespace AI.DaDataProxy.Validators;

/// <summary>
/// Предоставляет методы для валидации ИНН (Идентификационного номера налогоплательщика).
/// </summary>
public static class InnValidator
{
    /// <summary>
    /// Скомпилированное регулярное выражение для проверки, состоит ли строка только из цифр.
    /// </summary>
    private static readonly Regex _digitsOnlyRegex = new(@"^\d+$", RegexOptions.Compiled);

    /// <summary>
    /// Проверяет, является ли переданная строка действительным ИНН.
    /// </summary>
    /// <param name="inn">Строка для проверки.</param>
    /// <returns>true, если строка является действительным ИНН; иначе false.</returns>
    public static bool IsValid(string? inn)
    {
        if (string.IsNullOrWhiteSpace(inn))
            return false;

        if (!_digitsOnlyRegex.IsMatch(inn))
            return false;

        return inn.Length switch
        {
            10 => IsValidInn10(inn),
            12 => IsValidInn12(inn),
            _ => false
        };
    }

    private static bool IsValidInn10(string inn)
    {
        int sum = 
            2 * int.Parse(inn[0].ToString()) +
            4 * int.Parse(inn[1].ToString()) +
            10 * int.Parse(inn[2].ToString()) +
            3 * int.Parse(inn[3].ToString()) +
            5 * int.Parse(inn[4].ToString()) +
            9 * int.Parse(inn[5].ToString()) +
            4 * int.Parse(inn[6].ToString()) +
            6 * int.Parse(inn[7].ToString()) +
            8 * int.Parse(inn[8].ToString());

        sum = (sum % 11) % 10;

        return sum == int.Parse(inn[9].ToString());
    }

    private static bool IsValidInn12(string inn)
    {
        int sum1 = 
            7 * int.Parse(inn[0].ToString()) +
            2 * int.Parse(inn[1].ToString()) +
            4 * int.Parse(inn[2].ToString()) +
            10 * int.Parse(inn[3].ToString()) +
            3 * int.Parse(inn[4].ToString()) +
            5 * int.Parse(inn[5].ToString()) +
            9 * int.Parse(inn[6].ToString()) +
            4 * int.Parse(inn[7].ToString()) +
            6 * int.Parse(inn[8].ToString()) +
            8 * int.Parse(inn[9].ToString());

        sum1 = (sum1 % 11) % 10;

        int sum2 = 
            3 * int.Parse(inn[0].ToString()) +
            7 * int.Parse(inn[1].ToString()) +
            2 * int.Parse(inn[2].ToString()) +
            4 * int.Parse(inn[3].ToString()) +
            10 * int.Parse(inn[4].ToString()) +
            3 * int.Parse(inn[5].ToString()) +
            5 * int.Parse(inn[6].ToString()) +
            9 * int.Parse(inn[7].ToString()) +
            4 * int.Parse(inn[8].ToString()) +
            6 * int.Parse(inn[9].ToString()) +
            8 * int.Parse(inn[10].ToString());

        sum2 = (sum2 % 11) % 10;

        return sum1 == int.Parse(inn[10].ToString()) && sum2 == int.Parse(inn[11].ToString());
    }
}
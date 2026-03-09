using MURQ.Domain.URQL.Locations;

using System.Text;

namespace MURQ.Domain.URQL.Lexing.EnumerableExtensions;

public static class EnumerableExtensions
{
    public static IEnumerable<char> ToEnumerableWithoutCarriageReturn(this IEnumerable<char> enumerable)
        => new EnumerableWithoutCarriageReturn(enumerable);

    public static IEnumerable<(char Character, Position Position)> ToPositionedEnumerable(this IEnumerable<char> enumerable)
        => new PositionedEnumerable(enumerable);

    public static IEnumerable<(char Character, Position Position)> ToEnumerableWithoutComments(this IEnumerable<(char Character, Position Position)> enumerable)
        => new EnumerableWithoutComments(enumerable);

    public static IEnumerable<(char Character, Position Position)> ToEnumerableWithoutLineContinuations(this IEnumerable<(char Character, Position Position)> enumerable)
        => new EnumerableWithoutLineContinuation(enumerable);

    public static IEnumerable<List<(char Character, Position Position)>> SplitByLineBreaks(this IEnumerable<(char Character, Position Position)> enumerable)
        => new EnumerableByLineBreaks(enumerable);

    public static bool IsLabelLine(this List<(char Character, Position Position)> sourceLine, out string? label)
    {
        IEnumerable<char> characters = sourceLine.Select(element => element.Character);

        bool isColonFound = false;
        StringBuilder textBuilder = new();

        foreach (char @char in characters)
        {
            if (!isColonFound)
            {
                // До двоеточия пропускаем только пробелы и табуляции
                if (@char == ' ' || @char == '\t')
                    continue;
                else if (@char == ':')
                    isColonFound = true;
                else // Символ не пробел/таб и не двоеточие — ошибка
                {
                    label = null;
                    return false;
                }
            }
            else
            {
                // После двоеточия собираем все символы
                textBuilder.Append(@char);
            }
        }

        // Проверяем, что двоеточие было найдено и после него есть текст
        label = textBuilder.ToString();
        return isColonFound && textBuilder.Length > 0;
    }
}
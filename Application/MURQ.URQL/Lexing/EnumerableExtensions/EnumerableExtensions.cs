using MURQ.Domain.Quests.Locations;

using System.Text;

namespace MURQ.URQL.Lexing.EnumerableExtensions;

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

    public static (bool IsLabel, string? Label) TryExtractLabel(this List<(char Character, Position Position)> sourceLine)
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
                else
                    return (false, null); // Символ не пробел/таб и не двоеточие — ошибка
            }
            else
            {
                // После двоеточия собираем все символы
                textBuilder.Append(@char);
            }
        }

        // Проверяем, что двоеточие было найдено и после него есть текст
        return (isColonFound && textBuilder.Length > 0, textBuilder.ToString());
    }
}
using MURQ.Domain.URQL.Locations;

using System.Text;

namespace MURQ.Domain.URQL.Lexing.CharacterEnumerableExtensions;

public static class CharacterEnumerableExtensions
{
    public static IEnumerable<char> ToEnumerableWithoutCarriageReturn(this IEnumerable<char> enumerable)
        => new CharacterEnumerableWithoutCarriageReturn(enumerable);

    public static IEnumerable<PositionedCharacter> ToPositionedEnumerable(this IEnumerable<char> enumerable)
        => new PositionedEnumerable(enumerable);

    public static IEnumerable<PositionedCharacter> ToEnumerableWithoutComments(this IEnumerable<PositionedCharacter> enumerable)
        => new CharacterEnumerableWithoutComments(enumerable);

    public static IEnumerable<PositionedCharacter> ToEnumerableWithoutLineContinuations(this IEnumerable<PositionedCharacter> enumerable)
        => new CharacterEnumerableWithoutLineContinuation(enumerable);

    public static IEnumerable<List<PositionedCharacter>> SplitByLineBreaks(this IEnumerable<PositionedCharacter> enumerable)
        => new CharacterEnumerableByLineBreaks(enumerable);

    public static bool IsLabelLine(this List<PositionedCharacter> sourceLine, out string? label)
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

    public static string ToPlainString(this IEnumerable<PositionedCharacter> positionedCharacters) => string.Join(null, positionedCharacters.Select(pc => pc.Character));

    public static IEnumerable<OriginatedCharacter> AsOriginatedCharacters(this IEnumerable<PositionedCharacter> positionedCharacters)
        => positionedCharacters.Select(positionedCharacter => (OriginatedCharacter)positionedCharacter);
}
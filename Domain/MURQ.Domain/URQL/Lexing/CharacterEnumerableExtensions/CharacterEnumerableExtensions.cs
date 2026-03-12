using MURQ.Domain.URQL.Locations;

using System.Diagnostics.CodeAnalysis;
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

    public static bool IsLabelLine(this List<PositionedCharacter> sourceLine, [NotNullWhen(true)] out string? label)
    {
        bool isColonFound = false;
        StringBuilder labelTextBuilder = new();

        foreach (PositionedCharacter positionedCharacter in sourceLine)
        {
            if (!isColonFound)
            {
                // До двоеточия пропускаем только пробелы и табуляции
                if (positionedCharacter.Character is ' ' or '\t')
                    continue;
                else if (positionedCharacter.Character == ':')
                    isColonFound = true;
                else // Символ не пробел/таб и не двоеточие — не метка
                {
                    label = null;
                    return false;
                }
            }
            else
            {
                // После двоеточия собираем все символы
                labelTextBuilder.Append(positionedCharacter.Character);
            }
        }

        // Проверяем, что двоеточие было найдено и после него есть текст
        label = isColonFound ? labelTextBuilder.ToString() : null;
        return isColonFound;
    }

    public static string ToPlainString(this IEnumerable<PositionedCharacter> positionedCharacters) => string.Concat(positionedCharacters.Select(pc => pc.Character));
    public static string ToPlainString(this IEnumerable<OriginatedCharacter> originatedCharacters) => string.Concat(originatedCharacters.Select(pc => pc.Character));

    public static IEnumerable<OriginatedCharacter> AsOriginatedCharacters(this IEnumerable<PositionedCharacter> positionedCharacters)
        => positionedCharacters.Select(positionedCharacter => (OriginatedCharacter)positionedCharacter);
}
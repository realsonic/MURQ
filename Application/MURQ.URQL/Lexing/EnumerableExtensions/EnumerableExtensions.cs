using MURQ.URQL.Locations;

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

    public static IEnumerable<IEnumerable<(char Character, Position Position)>> SplitByLineBreaks(this IEnumerable<(char Character, Position Position)> enumerable)
        => new EnumerableByLineBreaks(enumerable);
}
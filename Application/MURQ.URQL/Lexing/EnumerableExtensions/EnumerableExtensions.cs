using MURQ.URQL.Locations;

namespace MURQ.URQL.Lexing.EnumerableExtensions;

public static class EnumerableExtensions
{
    public static IEnumerable<char> ToEnumerableWithoutCarriageReturn(this IEnumerable<char> enumerable)
        => new EnumerableWithoutCarriageReturn(enumerable);

    public static IEnumerable<(char, Position)> ToPositionedEnumerable(this IEnumerable<char> enumerable)
        => new PositionedEnumerable(enumerable);

    public static IEnumerable<(char, Position)> ToEnumerableWithoutComments(this IEnumerable<(char, Position)> enumerable)
        => new EnumerableWithoutComments(enumerable);
}
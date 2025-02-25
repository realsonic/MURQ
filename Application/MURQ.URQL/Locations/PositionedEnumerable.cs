using System.Collections;

namespace MURQ.URQL.Locations;

public class PositionedEnumerable(IEnumerable<char> enumerable) : IEnumerable<(char, Position)>
{
    IEnumerator<(char, Position)> IEnumerable<(char, Position)>.GetEnumerator()
    {
        return Enumerate().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Enumerate().GetEnumerator();
    }

    private IEnumerable<(char, Position)> Enumerate()
    {
        Position position = Position.Initial;

        foreach (char character in enumerable)
        {
            yield return (character, position);

            switch (character)
            {
                case '\n':
                    position = position.NewLine();
                    break;
                case not '\r':
                    position = position.AddColumn();
                    break;
            }
        }
    }
}

public static class PositionedEnumerableExtensions
{
    public static PositionedEnumerable ToPositionedEnumerable(this IEnumerable<char> enumerable) => new(enumerable);
}
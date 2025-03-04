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
            if (character is '\r') // трактуем как бесполезный символ, игнорируем
                continue;

            yield return (character, position);

            position = character switch
            {
                '\n' => position.NewLine(),
                _ => position.AddColumn(),
            };
        }
    }
}

public static class PositionedEnumerableExtensions
{
    public static PositionedEnumerable ToPositionedEnumerable(this IEnumerable<char> enumerable) => new(enumerable);
}
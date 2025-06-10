using MURQ.URQL.Locations;

using System.Collections;

namespace MURQ.URQL.Lexing.EnumerableExtensions;

public class PositionedEnumerable(IEnumerable<char> enumerable) : IEnumerable<(char, Position)>
{
    IEnumerator<(char, Position)> IEnumerable<(char, Position)>.GetEnumerator() => Enumerate().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => Enumerate().GetEnumerator();

    private IEnumerable<(char, Position)> Enumerate()
    {
        Position position = Position.Initial;

        foreach (char character in enumerable)
        {
            yield return (character, position);

            position = character switch
            {
                '\n' => position.NewLine(),
                _ => position.AddColumn(),
            };
        }
    }
}
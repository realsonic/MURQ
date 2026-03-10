using MURQ.Domain.URQL.Locations;

using System.Collections;

namespace MURQ.Domain.URQL.Lexing.CharacterEnumerableExtensions;

public class PositionedEnumerable(IEnumerable<char> enumerable) : IEnumerable<PositionedCharacter>
{
    IEnumerator<PositionedCharacter> IEnumerable<PositionedCharacter>.GetEnumerator() => Enumerate().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => Enumerate().GetEnumerator();

    private IEnumerable<PositionedCharacter> Enumerate()
    {
        Position position = Position.Initial;

        foreach (char character in enumerable)
        {
            yield return new(character, position);

            position = character switch
            {
                '\n' => position.NewLine(),
                _ => position.AddColumn(),
            };
        }
    }
}
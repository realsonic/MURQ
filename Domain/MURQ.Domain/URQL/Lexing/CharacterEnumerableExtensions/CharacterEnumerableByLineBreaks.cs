using MURQ.Domain.URQL.Locations;

using System.Collections;

namespace MURQ.Domain.URQL.Lexing.CharacterEnumerableExtensions;
public class CharacterEnumerableByLineBreaks(IEnumerable<PositionedCharacter> enumerable) : IEnumerable<List<PositionedCharacter>>
{
    public IEnumerator<List<PositionedCharacter>> GetEnumerator() => Enumerate().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => Enumerate().GetEnumerator();

    private IEnumerable<List<PositionedCharacter>> Enumerate()
    {
        List<PositionedCharacter> line = [];

        foreach ((char character, Position position) in enumerable)
        {
            if (character is '\n')
            {
                if (line.Count > 0)
                {
                    yield return [.. line];
                    line.Clear();
                }
            }
            else
            {
                line.Add(new(character, position));
            }
        }

        if (line.Count > 0)
        {
            yield return [.. line];
        }
    }
}

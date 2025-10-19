using MURQ.URQL.Locations;

using System.Collections;

namespace MURQ.URQL.Lexing.EnumerableExtensions;
public class EnumerableByLineBreaks(IEnumerable<(char Character, Position Position)> enumerable) : IEnumerable<IEnumerable<(char Character, Position Position)>>
{
    public IEnumerator<IEnumerable<(char Character, Position Position)>> GetEnumerator() => Enumerate().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => Enumerate().GetEnumerator();

    private IEnumerable<IEnumerable<(char Character, Position Position)>> Enumerate()
    {
        List<(char Character, Position Position)> line = [];

        foreach ((char character, Position position) in enumerable)
        {
            if (character is '\n')
            {
                yield return [.. line];
                line.Clear();
            }
            else
            {
                line.Add((character, position));
            }
        }

        if (line.Count > 0)
        {
            yield return [.. line];
        }
    }
}

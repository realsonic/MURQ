using System.Collections;

namespace MURQ.URQL.Lexing.EnumerableExtensions;

public class EnumerableWithoutCarriageReturn(IEnumerable<char> enumerable) : IEnumerable<char>
{
    IEnumerator<char> IEnumerable<char>.GetEnumerator() => Enumerate().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => Enumerate().GetEnumerator();

    private IEnumerable<char> Enumerate()
    {
        foreach (char character in enumerable)
        {
            if (character is '\r') // трактуем как бесполезный символ, игнорируем
                continue;

            yield return character;
        }
    }
}

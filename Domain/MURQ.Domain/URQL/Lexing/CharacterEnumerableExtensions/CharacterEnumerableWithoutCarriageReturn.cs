using System.Collections;

namespace MURQ.Domain.URQL.Lexing.CharacterEnumerableExtensions;

public class CharacterEnumerableWithoutCarriageReturn(IEnumerable<char> enumerable) : IEnumerable<char>
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

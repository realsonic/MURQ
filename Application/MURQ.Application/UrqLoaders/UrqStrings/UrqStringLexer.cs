using MURQ.Application.UrqLoaders.UrqStrings.Tokens;

using System.Text;

namespace MURQ.Application.UrqLoaders.UrqStrings;

public class UrqStringLexer
{
    public IEnumerable<Token> Scan(IEnumerable<char> source)
    {
        textStringBuilder.Clear();

        foreach (char character in source)
        {
            switch (character)
            {
                case '#':
                    if (textStringBuilder.Length > 0)
                    {
                        yield return new TextToken(textStringBuilder.ToString());
                        textStringBuilder.Clear();
                    }
                    yield return new SubstitutionStartToken();
                    break;

                case '$':
                    if (textStringBuilder.Length > 0)
                    {
                        yield return new TextToken(textStringBuilder.ToString());
                        textStringBuilder.Clear();
                    }
                    yield return new SubstitutionStopToken();
                    break;

                default:
                    textStringBuilder.Append(character);
                    break;
            }
        }

        if (textStringBuilder.Length > 0)
        {
            yield return new TextToken(textStringBuilder.ToString());
            textStringBuilder.Clear();
        }
    }

    private readonly StringBuilder textStringBuilder = new();
}

using System.Text;

namespace MURQ.Application.UrqLoaders.UrqStrings;

internal class UrqStringParser
{
    internal static IEnumerable<Token> ParseTokens(string sourceString)
    {
        StringBuilder stringBuilder = new();

        foreach (char character in sourceString)
        {
            switch (character)
            {
                case '#':
                    if (stringBuilder.Length > 0)
                    {
                        yield return new TextToken(stringBuilder.ToString());
                        stringBuilder.Clear();
                    }
                    yield return new BeginInterpolationToken();
                    break;

                case '$':
                    if (stringBuilder.Length > 0)
                    {
                        yield return new TextToken(stringBuilder.ToString());
                        stringBuilder.Clear();
                    }
                    yield return new EndInterpolationToken();
                    break;

                default:
                    stringBuilder.Append(character);
                    break;
            }
        }

        if (stringBuilder.Length > 0)
        {
            yield return new TextToken(stringBuilder.ToString());
            stringBuilder.Clear();
        }
    }
}

internal abstract record Token();
internal record TextToken(string Text) : Token;
internal record BeginInterpolationToken() : Token();
internal record EndInterpolationToken() : Token();
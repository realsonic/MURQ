using MURQ.Application.UrqLoaders.UrqStrings.Tokens;

using System.Text;

namespace MURQ.Application.UrqLoaders.UrqStrings;

public class UrqStringLexer
{
    public IEnumerable<Token> Scan(IEnumerable<char> source)
    {
        ClearText();

        foreach (char character in source)
        {
            switch (character)
            {
                case '#':
                    if (HasText())
                        yield return new TextToken(PopText());
                    yield return new SubstitutionStartToken();
                    break;

                case '$':
                    if (HasText())
                        yield return new TextToken(PopText());
                    yield return new SubstitutionStopToken();
                    break;

                default:
                    PushCharacter(character);
                    break;
            }
        }

        if (HasText())
            yield return new TextToken(PopText());
    }

    private void ClearText() => textStringBuilder.Clear();

    private bool HasText() => textStringBuilder.Length > 0;

    private void PushCharacter(char character) => textStringBuilder.Append(character);

    private string PopText()
    {
        var text = textStringBuilder.ToString();
        textStringBuilder.Clear();
        return text;
    }

    private readonly StringBuilder textStringBuilder = new();
}

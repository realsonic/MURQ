using MURQ.Application.UrqLoaders.UrqStrings.Tokens;

using System.Text;

using static MURQ.Application.UrqLoaders.UrqStrings.Tokens.SubstitutionStartToken;

namespace MURQ.Application.UrqLoaders.UrqStrings;

public class UrqStringLexer
{
    public IEnumerable<Token> Scan(IEnumerable<char> source)
    {
        stringBuilder.Clear();
        var lexemState = LexemState.StringInProgress;

        foreach (char character in source)
        {
            switch (lexemState)
            {
                case LexemState.StringInProgress:
                    switch (character)
                    {
                        case '#':
                            if (HasString())
                                yield return PopStringAsToken();
                            lexemState = LexemState.SubstitutionStartMet;
                            break;

                        case '$':
                            if (HasString())
                                yield return PopStringAsToken();
                            yield return new SubstitutionStopToken();
                            break;

                        default:
                            PushStringCharacter(character);
                            break;
                    }
                    break;

                case LexemState.SubstitutionStartMet:
                    if (character is '%')
                    {
                        yield return new SubstitutionStartToken(ModifierEnum.AsString);
                    }
                    else
                    {
                        yield return new SubstitutionStartToken(ModifierEnum.None);
                        PushStringCharacter(character);
                    }
                    lexemState = LexemState.StringInProgress;
                    break;

                default: throw new NotImplementedException($"Статус лексемы подстановки {lexemState} пока не обрабатывается.");
            }
        }

        if (HasString())
            yield return PopStringAsToken();
    }

    private bool HasString() => stringBuilder.Length > 0;

    private void PushStringCharacter(char character) => stringBuilder.Append(character);

    private StringToken PopStringAsToken()
    {
        var text = stringBuilder.ToString();
        stringBuilder.Clear();
        return new StringToken(text);
    }

    private readonly StringBuilder stringBuilder = new();

    private enum LexemState
    {
        StringInProgress,
        SubstitutionStartMet,
    }
}

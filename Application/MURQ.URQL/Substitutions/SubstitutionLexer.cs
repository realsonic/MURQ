using MURQ.URQL.Locations;
using MURQ.URQL.Substitutions.Tokens;

using System.Text;

using static MURQ.URQL.Substitutions.Tokens.SubstitutionStartToken;

namespace MURQ.URQL.Substitutions;
public class SubstitutionLexer
{
    internal IEnumerable<Token> Scan(IEnumerable<(char Character, Position Position)> line)
    {
        stringBuilder.Clear();
        var lexemState = LexemState.StringInProgress;

        foreach (char character in line.Select(@char => @char.Character))
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
                            PushCharacter(character);
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
                        PushCharacter(character);
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

    private void PushCharacter(char character) => stringBuilder.Append(character);

    private StringToken PopStringAsToken()
    {
        var @string = stringBuilder.ToString();
        stringBuilder.Clear();
        return new StringToken(@string);
    }

    private readonly StringBuilder stringBuilder = new();

    private enum LexemState
    {
        StringInProgress,
        SubstitutionStartMet,
    }
}

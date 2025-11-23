using MURQ.URQL.Locations;
using MURQ.URQL.Substitutions.Tokens;

using static MURQ.URQL.Substitutions.Tokens.SubstitutionStartToken;

namespace MURQ.URQL.Substitutions;
public class SubstitutionLexer
{
    internal IEnumerable<Token> Scan(IEnumerable<(char Character, Position Position)> line)
    {
        characterList.Clear();
        var lexemState = LexemState.StringInProgress;
        Position? substitutionStartPosition = null;

        foreach ((char Character, Position Position) in line)
        {
            switch (lexemState)
            {
                case LexemState.StringInProgress:
                    switch (Character)
                    {
                        case '#':
                            if (HasString())
                                yield return PopStringAsToken();
                            lexemState = LexemState.SubstitutionStartMet;
                            substitutionStartPosition = Position;
                            break;

                        case '$':
                            if (HasString())
                                yield return PopStringAsToken();
                            yield return new SubstitutionStopToken(Location.StartAt(Position));
                            break;

                        default:
                            PushCharacter((Character, Position));
                            break;
                    }
                    break;

                case LexemState.SubstitutionStartMet:
                    Position start = substitutionStartPosition ?? throw new InvalidOperationException($"Неожиданно не задана стартовая позиция для состояния {lexemState}.");
                    
                    if (Character is '%')
                    {
                        yield return new SubstitutionStartToken(ModifierEnum.AsString, new Location(start, Position));
                    }
                    else
                    {
                        yield return new SubstitutionStartToken(ModifierEnum.None, new Location(start, start));
                        PushCharacter((Character, Position));
                    }
                    lexemState = LexemState.StringInProgress;
                    substitutionStartPosition = null;
                    break;

                default: throw new NotImplementedException($"Статус лексемы подстановки {lexemState} пока не обрабатывается.");
            }
        }

        if (HasString())
            yield return PopStringAsToken();
    }

    private bool HasString() => characterList.Count > 0;

    private void PushCharacter((char Character, Position Position) @char) => characterList.Add(@char);

    private StringToken PopStringAsToken()
    {
        var @string = new string([.. characterList.Select(@char => @char.Character)]);
        var start = characterList.First().Position;
        var stop = characterList.Last().Position;

        characterList.Clear();
        
        return new StringToken(@string, new Location(start, stop));
    }

    private readonly List<(char Character, Position Position)> characterList = [];

    private enum LexemState
    {
        StringInProgress,
        SubstitutionStartMet,
    }
}

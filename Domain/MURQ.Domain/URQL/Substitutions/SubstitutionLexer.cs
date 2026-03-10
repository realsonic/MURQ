using MURQ.Domain.URQL.Locations;
using MURQ.Domain.URQL.Substitutions.Tokens;

using static MURQ.Domain.URQL.Substitutions.Tokens.SubstitutionStartToken;

namespace MURQ.Domain.URQL.Substitutions;
public class SubstitutionLexer
{
    internal IEnumerable<Token> Scan(IEnumerable<PositionedCharacter> line)
    {
        characterList.Clear();
        var lexemState = LexemState.StringInProgress;
        Position? substitutionStartPosition = null;

        foreach (PositionedCharacter positionedCharacter in line)
        {
            switch (lexemState)
            {
                case LexemState.StringInProgress:
                    switch (positionedCharacter.Character)
                    {
                        case '#':
                            if (HasString())
                                yield return PopStringAsToken();
                            lexemState = LexemState.SubstitutionStartMet;
                            substitutionStartPosition = positionedCharacter.Position;
                            break;

                        case '$':
                            if (HasString())
                                yield return PopStringAsToken();
                            yield return new SubstitutionStopToken(Location.StartAt(positionedCharacter.Position));
                            break;

                        default:
                            PushCharacter(positionedCharacter);
                            break;
                    }
                    break;

                case LexemState.SubstitutionStartMet:
                    Position start = substitutionStartPosition ?? throw new InvalidOperationException($"Неожиданно не задана стартовая позиция для состояния {lexemState}.");
                    
                    if (positionedCharacter.Character is '%')
                    {
                        yield return new SubstitutionStartToken(ModifierEnum.AsString, new Location(start, positionedCharacter.Position));
                    }
                    else
                    {
                        yield return new SubstitutionStartToken(ModifierEnum.None, new Location(start, start));
                        PushCharacter(positionedCharacter);
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

    private void PushCharacter(PositionedCharacter positionedCharacter) => characterList.Add(positionedCharacter);

    private StringToken PopStringAsToken()
    {
        Position start = characterList.First().Position;
        Position stop = characterList.Last().Position;
        List<PositionedCharacter> sourceCharacters = [.. characterList];
        
        characterList.Clear();
        
        return new StringToken(sourceCharacters, new Location(start, stop));
    }

    private readonly List<PositionedCharacter> characterList = [];

    private enum LexemState
    {
        StringInProgress,
        SubstitutionStartMet,
    }
}

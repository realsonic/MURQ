using MURQ.Domain.Games;
using MURQ.Domain.Games.Values;
using MURQ.Domain.Quests.Expressions;
using MURQ.Domain.URQL.Lexing;
using MURQ.Domain.URQL.Locations;

namespace MURQ.Domain.URQL.Interpretation;

public class ExpressionCalculator(IEnumerable<OriginatedCharacter> sourceCharacters, IGameContext gameContext) : UrqlParser(new UrqlLexer(sourceCharacters).Scan())
{

    public Value? Calculate()
    {
        MoveToNextTerminal();

        if (Lookahead is null)
        {
            return null;
        }

        Expression expression = ParseValueExpression();

        return expression.Calculate(gameContext);
    }
}

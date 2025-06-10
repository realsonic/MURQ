using MURQ.Domain.Games;
using MURQ.Domain.Games.Values;

namespace MURQ.Domain.Quests.Expressions;

public class RelationExpression : Expression
{
    public required Expression LeftExpression { get; init; }

    public required Expression RightExpression { get; init; }

    public override Value Calculate(IGameContext gameContext)
    {
        Value leftValue = LeftExpression.Calculate(gameContext);
        Value rightValue = RightExpression.Calculate(gameContext);

        return new DecimalValue(leftValue == rightValue ? 1 : 0);
    }
}

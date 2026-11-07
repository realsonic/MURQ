using MURQ.Domain.Games;
using MURQ.Domain.Games.Values;

using System.Diagnostics;

namespace MURQ.Domain.Quests.Expressions.Math;

[DebuggerDisplay("{LeftExpression} - {RightExpression}")]
public class SubstractionExpression : Expression
{
    public required Expression LeftExpression { get; init; }

    public required Expression RightExpression { get; init; }

    public override Value Calculate(IGameContext gameContext)
    {
        Value leftValue = LeftExpression.Calculate(gameContext);
        Value rightValue = RightExpression.Calculate(gameContext);
        return new NumberValue(leftValue.AsDecimal - rightValue.AsDecimal);
    }
}
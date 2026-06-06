using MURQ.Domain.Games;
using MURQ.Domain.Games.Values;

using System.Diagnostics;

namespace MURQ.Domain.Quests.Expressions;

[DebuggerDisplay("{LeftExpression} + {RightExpression}")]
public class AdditionExpression : Expression
{
    public required Expression LeftExpression { get; init; }

    public required Expression RightExpression { get; init; }

    public override Value Calculate(IGameContext gameContext)
    {
        Value leftValue = LeftExpression.Calculate(gameContext);
        Value rightValue = RightExpression.Calculate(gameContext);

        if (leftValue is StringValue leftStringValue && rightValue is StringValue rightStringValue)
        {
            return new StringValue(leftStringValue.Value + rightStringValue.Value);
        }

        return new NumberValue(leftValue.AsDecimal + rightValue.AsDecimal);
    }
}

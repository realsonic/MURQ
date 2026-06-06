using MURQ.Domain.Games;
using MURQ.Domain.Games.Values;

using System.Diagnostics;

namespace MURQ.Domain.Quests.Expressions;

[DebuggerDisplay("{LeftExpression} / {RightExpression}")]
public class DivisionExpression : Expression
{
    public required Expression LeftExpression { get; init; }

    public required Expression RightExpression { get; init; }

    public override Value Calculate(IGameContext gameContext)
    {
        Value leftValue = LeftExpression.Calculate(gameContext);

        Value rightValue = RightExpression.Calculate(gameContext);
        decimal divisor = rightValue.AsDecimal;
        if (divisor == 0) return new NumberValue(0); // избегаем деления на ноль

        return new NumberValue(leftValue.AsDecimal / divisor);
    }
}
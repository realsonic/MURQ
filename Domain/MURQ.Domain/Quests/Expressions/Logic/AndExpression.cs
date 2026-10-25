using MURQ.Domain.Games;
using MURQ.Domain.Games.Values;

using System.Diagnostics;

namespace MURQ.Domain.Quests.Expressions.Logic;

[DebuggerDisplay("{LeftExpression} and {RightExpression}")]
public class AndExpression : Expression
{
    public required Expression LeftExpression { get; init; }

    public required Expression RightExpression { get; init; }

    public override Value Calculate(IGameContext gameContext)
    {
        Value leftResult = LeftExpression.Calculate(gameContext);
        Value rightResult = RightExpression.Calculate(gameContext);

        bool result = leftResult.AsDecimal != 0 && rightResult.AsDecimal != 0;

        return new NumberValue(result ? 1 : 0);
    }
}

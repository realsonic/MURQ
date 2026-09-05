using MURQ.Domain.Games;

using System.Diagnostics;

namespace MURQ.Domain.Quests.Expressions.Logic;

[DebuggerDisplay("{LeftExpression} or {RightExpression}")]
public class DisjunctionExpression : LogicExpression
{
    public required LogicExpression LeftExpression { get; init; }

    public required LogicExpression RightExpression { get; init; }

    public override bool Calculate(IGameContext gameContext)
    {
        bool leftResult = LeftExpression.Calculate(gameContext);
        bool rightResult = RightExpression.Calculate(gameContext);

        return leftResult || rightResult;
    }
}

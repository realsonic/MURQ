using MURQ.Domain.Games;

using System.Diagnostics;

namespace MURQ.Domain.Quests.Expressions.Logic;

[DebuggerDisplay("{LeftExpression} and {RightExpression}")]
public class ConjunctionExpression : LogicExpression
{
    public required LogicExpression LeftExpression { get; init; }

    public required LogicExpression RightExpression { get; init; }

    public override bool Calculate(IGameContext gameContext)
    {
        bool leftResult = LeftExpression.Calculate(gameContext);
        bool rightResult = RightExpression.Calculate(gameContext);
        
        return leftResult && rightResult;
    }
}

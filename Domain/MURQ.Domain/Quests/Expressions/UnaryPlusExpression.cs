using MURQ.Domain.Games;
using MURQ.Domain.Games.Values;

using System.Diagnostics;

namespace MURQ.Domain.Quests.Expressions;

[DebuggerDisplay("+{Expression}")]
public class UnaryPlusExpression : Expression
{
    public required Expression Expression { get; init; }

    public override Value Calculate(IGameContext gameContext)
    {
        Value value = Expression.Calculate(gameContext);
        return value;
    }
}

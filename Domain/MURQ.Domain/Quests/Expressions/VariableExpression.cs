using MURQ.Domain.Games;
using MURQ.Domain.Games.Values;

using System.Diagnostics;

namespace MURQ.Domain.Quests.Expressions;

[DebuggerDisplay("{Name,nq}")]
public class VariableExpression : Expression
{
    public required string Name { get; init; }

    public override Value Calculate(IGameContext gameContext)
    {
        Value? value = gameContext.GetVariableValue(Name);

        return value ?? new NumberValue(0);
    }
}
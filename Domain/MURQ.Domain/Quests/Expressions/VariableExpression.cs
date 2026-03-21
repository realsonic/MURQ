using MURQ.Domain.Games;
using MURQ.Domain.Games.Values;
using MURQ.Domain.Games.Variables;

using System.Diagnostics;

namespace MURQ.Domain.Quests.Expressions;

[DebuggerDisplay("{Name,nq}")]
public class VariableExpression : Expression
{
    public required string Name { get; init; }

    public override Value Calculate(IGameContext gameContext)
    {
        Variable? variable = gameContext.GetVariable(Name);

        return variable?.Value ?? new NumberValue(0);
    }
}
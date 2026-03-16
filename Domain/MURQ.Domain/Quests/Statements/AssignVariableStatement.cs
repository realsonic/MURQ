using MURQ.Domain.Games;
using MURQ.Domain.Games.Values;
using MURQ.Domain.Quests.Expressions;

using System.Diagnostics;

namespace MURQ.Domain.Quests.Statements;

[DebuggerDisplay("{VariableName,nq} = {Expression,nq}")]
public class AssignVariableStatement : Statement
{
    public required string VariableName { get; init; }

    public required Expression Expression { get; init; }

    public override Task RunAsync(IGameContext gameContext, CancellationToken cancellationToken)
    {
        Value value = Expression.Calculate(gameContext);
        gameContext.AssignVariable(VariableName, value);
        
        return Task.CompletedTask;
    }
}

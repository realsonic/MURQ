using MURQ.Domain.Games;
using MURQ.Domain.Games.Values;
using MURQ.Domain.Quests.Expressions;

namespace MURQ.Domain.Quests.Statements;
public class AssignVariableStatement : Statement
{
    public required string VariableName { get; init; }

    public required Expression Expression { get; init; }

    public override Task RunAsync(IGameContext gameContext)
    {
        Value value = Expression.Calculate(gameContext);
        gameContext.AssignVariable(VariableName, value);
        
        return Task.CompletedTask;
    }
}

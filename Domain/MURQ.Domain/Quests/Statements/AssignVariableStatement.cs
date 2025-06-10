using MURQ.Domain.Games;

namespace MURQ.Domain.Quests.Statements;
public class AssignVariableStatement : Statement
{
    public required string VariableName { get; init; }

    public decimal Value { get; init; }

    public override void Run(IGameContext gameContext)
    {
        gameContext.AssignVariable(VariableName, Value);
    }
}

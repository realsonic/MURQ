using MURQ.Domain.Games;
using MURQ.Domain.Games.Values;
using MURQ.Domain.Quests.Expressions;

namespace MURQ.Domain.Quests.Statements;
public class IfStatement : Statement
{
    public required Expression Condition { get; init; }

    public required Statement ThenStatement { get; init; }

    public override async Task RunAsync(IGameContext gameContext)
    {
        Value value = Condition.Calculate(gameContext);
        if (value.AsDecimal != 0)
        {
            await ThenStatement.RunAsync(gameContext);
        }
    }
}

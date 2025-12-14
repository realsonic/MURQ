using MURQ.Domain.Games;

using System.Diagnostics;

namespace MURQ.Domain.Quests.Statements;

[DebuggerDisplay("end")]
public class EndStatement : Statement
{
    public override Task RunAsync(IGameContext gameContext, CancellationToken cancellationToken)
    {
        gameContext.EndLocation();

        return Task.CompletedTask;
    }
}
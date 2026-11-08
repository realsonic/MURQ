using MURQ.Domain.Games;

using System.Diagnostics;

namespace MURQ.Domain.Quests.Statements;

[DebuggerDisplay("forget_procs")]
public class ForgetProcsStatement : Statement
{
    public override Task RunAsync(IGameContext gameContext, CancellationToken cancellationToken)
    {
        gameContext.ForgetProcs();

        return Task.CompletedTask;
    }
}

using MURQ.Domain.Games;

using System.Diagnostics;

namespace MURQ.Domain.Quests.Statements;

[DebuggerDisplay("proc {Label,nq}")]
public class ProcStatement : Statement
{
    public required string Label { get; init; }

    public override Task RunAsync(IGameContext gameContext, CancellationToken cancellationToken)
    {
        gameContext.Proc(Label);

        return Task.CompletedTask;
    }
}

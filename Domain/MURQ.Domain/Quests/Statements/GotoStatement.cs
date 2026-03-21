using MURQ.Domain.Games;

using System.Diagnostics;

namespace MURQ.Domain.Quests.Statements;

[DebuggerDisplay("goto {Label,nq}")]
public class GotoStatement : Statement
{
    public required string Label { get; init; }

    public override Task RunAsync(IGameContext gameContext, CancellationToken cancellationToken)
    {
        gameContext.Goto(Label);

        return Task.CompletedTask;
    }
}

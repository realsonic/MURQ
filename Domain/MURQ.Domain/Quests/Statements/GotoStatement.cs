using MURQ.Domain.Games;

using System.Diagnostics;

namespace MURQ.Domain.Quests.Statements;

[DebuggerDisplay("goto {LabelStatement?.Label,nq}")]
public class GotoStatement : Statement
{
    public LabelStatement? LabelStatement { get; init; }

    public override Task RunAsync(IGameContext gameContext, CancellationToken cancellationToken)
    {
        gameContext.Goto(LabelStatement);

        return Task.CompletedTask;
    }
}

using System.Diagnostics;

using MURQ.Domain.Games;

namespace MURQ.Domain.Quests.Statements;

[DebuggerDisplay(":{Label,nq}")]
public class LabelStatement : Statement
{
    public required string Label { get; init; }

    public override Task RunAsync(IGameContext gameContext)
    {
        return Task.CompletedTask;
    }
}
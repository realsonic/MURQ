using System.Diagnostics;

using MURQ.Domain.Games;

namespace MURQ.Domain.Quests.Statements;

[DebuggerDisplay("btn {LabelStatement?.Label,nq},{Caption,nq}")]
public class ButtonStatement : Statement
{
    public required string Caption { get; init; }

    public LabelStatement? LabelStatement { get; init; }

    public override Task RunAsync(IGameContext gameContext, CancellationToken cancellationToken)
    {
        gameContext.AddButton(Caption, LabelStatement);

        return Task.CompletedTask;
    }
}
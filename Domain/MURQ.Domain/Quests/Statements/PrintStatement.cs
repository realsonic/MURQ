using MURQ.Domain.Games;

using System.Diagnostics;

namespace MURQ.Domain.Quests.Statements;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class PrintStatement : Statement
{
    public required string Text { get; init; }

    public bool IsNewLineAtEnd { get; init; }

    public override Task RunAsync(IGameContext gameContext, CancellationToken cancellationToken)
    {
        if (IsNewLineAtEnd)
            gameContext.PrintLine(Text);
        else
            gameContext.Print(Text);

        return Task.CompletedTask;
    }

    private string DebuggerDisplay => $"{(IsNewLineAtEnd ? "pln" : "p")} {Text}";
}
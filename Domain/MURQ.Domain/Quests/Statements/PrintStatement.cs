using MURQ.Domain.Games;
using MURQ.Domain.Quests.UrqStrings;

using System.Diagnostics;

namespace MURQ.Domain.Quests.Statements;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class PrintStatement : Statement
{
    public UrqString? UrqString { get; init; }

    public bool IsNewLineAtEnd { get; init; }

    public override Task RunAsync(IGameContext gameContext)
    {
        string? text = UrqString?.ToString(gameContext);

        if (IsNewLineAtEnd)
            gameContext.PrintLine(text);
        else
            gameContext.Print(text);

        return Task.CompletedTask;
    }

    private string DebuggerDisplay => $"{(IsNewLineAtEnd ? "pln" : "p")} {UrqString}";
}
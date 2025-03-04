using MURQ.Domain.Games;

using System.Diagnostics;

namespace MURQ.Domain.Quests.Statements;

[DebuggerDisplay("Print {Text,nq}, \\n = {IsNewLineAtEnd}")]
public class PrintStatement : Statement
{
    public string? Text { get; init; }

    public bool IsNewLineAtEnd { get; init; }

    public override void Run(IGameContext gameContext)
    {
        gameContext.PrintText(Text + (IsNewLineAtEnd ? "\n" : string.Empty));
    }
}
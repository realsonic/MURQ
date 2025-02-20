using System.Diagnostics;

using MURQ.Domain.Games;

namespace MURQ.Domain.Quests.Statements;

[DebuggerDisplay("p {Text,nq}")]
public class PrintStatement : Statement
{
    public string? Text { get; init; }

    public override void Run(IGameContext gameContext)
    {
        gameContext.PrintText(Text);
    }
}
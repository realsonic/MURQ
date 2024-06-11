using System.Diagnostics;

using MURQ.Domain.Games;

namespace MURQ.Domain.Quests.Instructions;

[DebuggerDisplay("p {Text,nq}")]
public class PrintInstruction : Instruction
{
    public string? Text { get; init; }

    public override void Run(IGameContext gameContext)
    {
        gameContext.PrintText(Text);
    }
}
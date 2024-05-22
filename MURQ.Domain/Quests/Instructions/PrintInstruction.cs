using MURQ.Domain.Games;

namespace MURQ.Domain.Quests.Instructions;

public class PrintInstruction : Instruction
{
    public string? Text { get; init; }

    public override void Run(RunningContext runningContext)
    {
        runningContext.CallPrintText(Text);
    }
}
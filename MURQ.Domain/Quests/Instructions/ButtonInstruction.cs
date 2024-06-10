using System.Diagnostics;

using MURQ.Domain.Games;

namespace MURQ.Domain.Quests.Instructions;

[DebuggerDisplay("btn {LabelInstruction?.Label,nq},{Caption,nq}")]
public class ButtonInstruction : Instruction
{
    public required string Caption { get; init; }

    public LabelInstruction? LabelInstruction { get; init; }

    public override void Run(RunningContext runningContext)
    {
        runningContext.AddButton(Caption, LabelInstruction);
    }
}
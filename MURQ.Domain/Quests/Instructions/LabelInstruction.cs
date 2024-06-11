using System.Diagnostics;

using MURQ.Domain.Games;

namespace MURQ.Domain.Quests.Instructions;

[DebuggerDisplay(":{Label,nq}")]
public class LabelInstruction : Instruction
{
    public required string Label { get; init; }

    public override void Run(IGameContext gameContext)
    {
        gameContext.ChangeLocation(Label);
    }
}
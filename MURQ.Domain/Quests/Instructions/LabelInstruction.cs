using System.Diagnostics;

using MURQ.Domain.Games;

namespace MURQ.Domain.Quests.Instructions;

[DebuggerDisplay(":{Label,nq}")]
public class LabelInstruction : Instruction
{
    public required string Label { get; init; }

    public override void Run(IGameContext gameContext)
    {
        // Инструкция-метка ничего не делает сама по себе, она лишь является маркером для перехода
        gameContext.ChangeLocation(Label);
    }
}
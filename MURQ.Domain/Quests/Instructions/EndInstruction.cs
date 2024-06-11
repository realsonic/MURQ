using MURQ.Domain.Games;

namespace MURQ.Domain.Quests.Instructions;

public class EndInstruction : Instruction
{
    public override void Run(IGameContext gameContext)
    {
        gameContext.End();
    }
}
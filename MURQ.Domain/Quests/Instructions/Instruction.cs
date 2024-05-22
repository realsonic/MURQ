using MURQ.Domain.Games;

namespace MURQ.Domain.Quests.Instructions;

public abstract class Instruction
{
    public abstract void Run(RunningContext runningContext);
}
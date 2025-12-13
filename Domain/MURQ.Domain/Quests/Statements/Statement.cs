using MURQ.Domain.Games;

namespace MURQ.Domain.Quests.Statements;

public abstract class Statement
{
    public abstract Task RunAsync(IGameContext gameContext);
}
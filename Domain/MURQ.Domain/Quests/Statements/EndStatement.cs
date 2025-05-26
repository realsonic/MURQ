using MURQ.Domain.Games;

namespace MURQ.Domain.Quests.Statements;

public class EndStatement : Statement
{
    public override void Run(IGameContext gameContext)
    {
        gameContext.EndLocation();
    }
}
using MURQ.Domain.Games;

namespace MURQ.Domain.Quests.Statements;
public class ClearScreenStatement : Statement
{
    public override void Run(IGameContext gameContext)
    {
        gameContext.ClearScreen();
    }
}

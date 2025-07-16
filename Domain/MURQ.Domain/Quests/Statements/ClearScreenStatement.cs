using MURQ.Domain.Games;

using System.Diagnostics;

namespace MURQ.Domain.Quests.Statements;

[DebuggerDisplay("cls")]
public class ClearScreenStatement : Statement
{
    public override void Run(IGameContext gameContext)
    {
        gameContext.ClearScreen();
    }
}

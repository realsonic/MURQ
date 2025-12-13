using MURQ.Domain.Games;

using System.Diagnostics;

namespace MURQ.Domain.Quests.Statements;

[DebuggerDisplay("cls")]
public class ClearScreenStatement : Statement
{
    public override Task RunAsync(IGameContext gameContext)
    {
        gameContext.ClearScreen();

        return Task.CompletedTask;
    }
}

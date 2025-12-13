using MURQ.Domain.Games;

using System.Diagnostics;

namespace MURQ.Domain.Quests.Statements;

[DebuggerDisplay("perkill")]
public class PerkillStatement : Statement
{
    public override Task RunAsync(IGameContext gameContext)
    {
        gameContext.Perkill();

        return Task.CompletedTask;
    }
}
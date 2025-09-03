using MURQ.Domain.Games;

using System.Diagnostics;

namespace MURQ.Domain.Quests.Statements;

[DebuggerDisplay("perkill")]
public class PerkillStatement : Statement
{
    public override void Run(IGameContext gameContext)
    {
        gameContext.Perkill();
    }
}
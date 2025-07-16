using MURQ.Domain.Games;

using System.Diagnostics;

namespace MURQ.Domain.Quests.Statements;

[DebuggerDisplay("end")]
public class EndStatement : Statement
{
    public override void Run(IGameContext gameContext)
    {
        gameContext.EndLocation();
    }
}
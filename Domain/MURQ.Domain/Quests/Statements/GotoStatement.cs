using MURQ.Domain.Games;

using System.Diagnostics;

namespace MURQ.Domain.Quests.Statements;

[DebuggerDisplay("goto {LabelStatement?.Label,nq}")]
public class GotoStatement : Statement
{
    public LabelStatement? LabelStatement { get; init; }

    public override void Run(IGameContext gameContext)
    {
        gameContext.Goto(LabelStatement);
    }
}

using System.Diagnostics;

using MURQ.Domain.Games;

namespace MURQ.Domain.Quests.Statements;

[DebuggerDisplay("btn {LabelInstruction?.Label,nq},{Caption,nq}")]
public class ButtonStatement : Statement
{
    public required string Caption { get; init; }

    public LabelStatement? LabelStatement { get; init; }

    public override void Run(IGameContext gameContext)
    {
        gameContext.AddButton(Caption, LabelStatement);
    }
}
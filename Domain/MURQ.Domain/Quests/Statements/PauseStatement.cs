using MURQ.Domain.Games;

using System.Diagnostics;

namespace MURQ.Domain.Quests.Statements;

[DebuggerDisplay("pause {Duration}")]
public class PauseStatement : Statement
{
    public int Duration { get; init; }

    public override async Task RunAsync(IGameContext gameContext)
    {
        await Task.Delay(Duration);
    }
}

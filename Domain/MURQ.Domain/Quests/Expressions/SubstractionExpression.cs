using MURQ.Domain.Games;
using MURQ.Domain.Games.Values;

using System.Diagnostics;

namespace MURQ.Domain.Quests.Expressions;

[DebuggerDisplay("{LeftExpression} - {RightExpression}")]
public class SubstractionExpression : Expression
{
    public required Expression LeftExpression { get; init; }

    public required Expression RightExpression { get; init; }

    public override Value Calculate(IGameContext gameContext)
    {
        throw new NotImplementedException();
    }
}
using MURQ.Domain.Games;
using MURQ.Domain.Games.Values;

using System.Diagnostics;

namespace MURQ.Domain.Quests.Expressions;

[DebuggerDisplay("{Value}")]
public class DecimalConstantExpression : Expression
{
    public required decimal Value { get; init; }

    public override Value Calculate(IGameContext gameContext) => new NumberValue(Value);
}
using MURQ.Domain.Games;
using MURQ.Domain.Games.Values;

namespace MURQ.Domain.Quests.Expressions;

public class DecimalConstantExpression : Expression
{
    public required decimal Value { get; init; }

    public override Value Calculate(IGameContext gameContext) => new DecimalValue(Value);
}
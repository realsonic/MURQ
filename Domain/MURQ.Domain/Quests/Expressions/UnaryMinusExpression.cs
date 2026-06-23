using MURQ.Domain.Games;
using MURQ.Domain.Games.Values;

using System.Diagnostics;

namespace MURQ.Domain.Quests.Expressions;

[DebuggerDisplay("-{Expression}")]
public class UnaryMinusExpression : Expression
{
    public required Expression Expression { get; init; }

    public override Value Calculate(IGameContext gameContext)
    {
        Value value = Expression.Calculate(gameContext);

        return value switch
        {
            NumberValue numberValue => new NumberValue(-numberValue.Value),
            StringValue stringValue => new NumberValue(-stringValue.Value.Length),
            _ => throw new NotImplementedException($"Тип значения {value.GetType()} ещё не обрабытвается.")
        };
    }
}

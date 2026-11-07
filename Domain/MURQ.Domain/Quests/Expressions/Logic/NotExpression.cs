using MURQ.Domain.Games;
using MURQ.Domain.Games.Values;

using System.Diagnostics;

namespace MURQ.Domain.Quests.Expressions.Logic;

[DebuggerDisplay("not {Expression}")]
public class NotExpression : Expression
{
    public required Expression Expression { get; init; }

    public override Value Calculate(IGameContext gameContext)
    {
        Value value = Expression.Calculate(gameContext);

        bool result = value.AsDecimal != 0;
        bool notResult = !result;

        return new NumberValue(notResult ? 1 : 0);
    }
}
using MURQ.Domain.Games;
using MURQ.Domain.Games.Values;

using System.Diagnostics;

namespace MURQ.Domain.Quests.Expressions.Logic;

[DebuggerDisplay("{LeftExpression} {RelationDebuggerDisplay} {RightExpression}")]
public class RelationExpression : LogicExpression
{
    public required RelationKind Kind { get; init; }

    public required Expression LeftExpression { get; init; }

    public required Expression RightExpression { get; init; }

    public override bool Calculate(IGameContext gameContext)
    {
        Value leftValue = LeftExpression.Calculate(gameContext);
        Value rightValue = RightExpression.Calculate(gameContext);

        return Kind switch
        {
            RelationKind.Equal => IsEqual(leftValue, rightValue),
            RelationKind.LessThan => leftValue.AsDecimal < rightValue.AsDecimal,
            RelationKind.LessThanOrEqual => leftValue.AsDecimal <= rightValue.AsDecimal,
            RelationKind.GreaterThan => leftValue.AsDecimal > rightValue.AsDecimal,
            RelationKind.GreaterThanOrEqual => leftValue.AsDecimal >= rightValue.AsDecimal,
            RelationKind.NotEqual => !IsEqual(leftValue, rightValue),
            _ => throw new NotImplementedException($"Тип отношения {Kind} ещё не обрабатывается."),
        };
    }

    private static bool IsEqual(Value leftValue, Value rightValue) => leftValue switch
    {
        NumberValue => leftValue.AsDecimal == rightValue.AsDecimal,
        StringValue => leftValue.AsString == rightValue.AsString,
        _ => throw new NotImplementedException($"Тип значения {leftValue.GetType()} ещё не обрабатывается.")
    };

    private string RelationDebuggerDisplay => Kind switch
    {
        RelationKind.Equal => "=",
        RelationKind.LessThan => "<",
        RelationKind.GreaterThan => ">",
        _ => throw new NotImplementedException($"Тип отношения {Kind} ещё не обрабатывается.")
    };

    public enum RelationKind
    {
        Equal,
        LessThan,
        LessThanOrEqual,
        GreaterThan,
        GreaterThanOrEqual,
        NotEqual
    }
}

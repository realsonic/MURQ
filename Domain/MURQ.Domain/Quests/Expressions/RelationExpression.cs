using MURQ.Domain.Games;
using MURQ.Domain.Games.Values;

using System.Diagnostics;

namespace MURQ.Domain.Quests.Expressions;

[DebuggerDisplay("{LeftExpression} {RelationDebuggerDisplay} {RightExpression}")]
public class RelationExpression : Expression
{
    public required RelationKind Kind { get; init; }

    public required Expression LeftExpression { get; init; }

    public required Expression RightExpression { get; init; }

    public override Value Calculate(IGameContext gameContext)
    {
        Value leftValue = LeftExpression.Calculate(gameContext);
        Value rightValue = RightExpression.Calculate(gameContext);

        bool result;
        switch (Kind)
        {
            case RelationKind.Equal:
                result = leftValue == rightValue;
                break;
            
            case RelationKind.LessThan:
                result = leftValue.AsDecimal < rightValue.AsDecimal;
                break;
            
            case RelationKind.GreaterThan:
                result = leftValue.AsDecimal > rightValue.AsDecimal;
                break;

            default:
                throw new NotImplementedException($"Тип отношения {Kind} ещё не обрабатывается.");
        };

        return new NumberValue(result ? 1 : 0);
    }

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
        GreaterThan
    }
}

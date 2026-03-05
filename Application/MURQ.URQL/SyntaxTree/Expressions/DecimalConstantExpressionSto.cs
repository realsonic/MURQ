using MURQ.Domain.Quests.Locations;

namespace MURQ.URQL.SyntaxTree.Expressions;

public record DecimalConstantExpressionSto(decimal Value, Location Location) : ExpressionSto
{
    public override Location Location { get; } = Location;
}
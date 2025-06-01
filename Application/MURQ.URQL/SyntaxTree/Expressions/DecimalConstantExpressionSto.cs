using MURQ.URQL.Locations;

namespace MURQ.URQL.SyntaxTree.Expressions;

public record DecimalConstantExpressionSto(decimal Value, Location Location) : ExpressionSto
{
    public override Location Location { get; } = Location;
}
using MURQ.URQL.Locations;

namespace MURQ.URQL.SyntaxTree.Expressions;

public record VariableExpressionSto(string VariableName, Location Location) : ExpressionSto
{
    public override Location Location { get; } = Location;
}

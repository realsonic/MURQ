using MURQ.URQL.Locations;

namespace MURQ.URQL.SyntaxTree.Expressions;

public record RelationExpressionSto(ExpressionSto LeftExpression, ExpressionSto RightExpression) : ExpressionSto
{
    public override Location Location => (LeftExpression.Location, RightExpression.Location);
}

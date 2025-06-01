using MURQ.URQL.Locations;
using MURQ.URQL.SyntaxTree.Expressions;

namespace MURQ.URQL.SyntaxTree.Statements;
public record IfStatementSto(ExpressionSto Condition, StatementSto ThenStatement) : StatementSto
{
    public override Location Location => (Condition.Location, ThenStatement.Location);
}

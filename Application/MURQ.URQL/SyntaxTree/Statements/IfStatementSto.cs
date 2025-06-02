using MURQ.URQL.Locations;
using MURQ.URQL.SyntaxTree.Expressions;

namespace MURQ.URQL.SyntaxTree.Statements;
public record IfStatementSto(ExpressionSto Condition, StatementSto ThenStatement, Position StartPosition) : StatementSto
{
    public override Location Location => new(StartPosition, ThenStatement.Location.End);
}

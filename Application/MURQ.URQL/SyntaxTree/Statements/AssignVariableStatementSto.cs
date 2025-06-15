using MURQ.URQL.Locations;
using MURQ.URQL.SyntaxTree.Expressions;

namespace MURQ.URQL.SyntaxTree.Statements;

public record AssignVariableStatementSto(string VariableName, ExpressionSto ExpressionSto, Location VariableLocation) : StatementSto
{
    public override Location Location => (VariableLocation, ExpressionSto.Location);
}

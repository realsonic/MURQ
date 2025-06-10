using MURQ.URQL.Locations;

namespace MURQ.URQL.SyntaxTree.Statements;

// Пока простая структура: имя переменной и числовой литерал
public record AssignVariableStatementSto(string VariableName, decimal Value, Location Location) : StatementSto
{
    public override Location Location { get; } = Location;
}

using MURQ.URQL.Locations;

namespace MURQ.URQL.SyntaxTree.Statements;

public record ButtonStatementSto(string Label, string Caption, Location Location) : StatementSto
{
    public override Location Location { get; } = Location;
}

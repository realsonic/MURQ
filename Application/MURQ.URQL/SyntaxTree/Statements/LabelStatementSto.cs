using MURQ.URQL.Locations;

namespace MURQ.URQL.SyntaxTree.Statements;

public record LabelStatementSto(string Label, Location Location) : StatementSto
{
    public override Location Location { get; } = Location;
}

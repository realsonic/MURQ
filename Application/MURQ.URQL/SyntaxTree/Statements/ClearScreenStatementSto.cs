using MURQ.URQL.Locations;

namespace MURQ.URQL.SyntaxTree.Statements;
public record ClearScreenStatementSto(Location Location) : StatementSto
{
    public override Location Location { get; } = Location;
}

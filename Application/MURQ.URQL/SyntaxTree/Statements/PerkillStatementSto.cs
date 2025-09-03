using MURQ.URQL.Locations;

namespace MURQ.URQL.SyntaxTree.Statements;

public record PerkillStatementSto(Location Location) : StatementSto
{
    public override Location Location { get; } = Location;
}

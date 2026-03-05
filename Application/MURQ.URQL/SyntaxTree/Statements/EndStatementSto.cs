using MURQ.Domain.Quests.Locations;

namespace MURQ.URQL.SyntaxTree.Statements;

public record EndStatementSto(Location Location) : StatementSto
{
    public override Location Location { get; } = Location;
}

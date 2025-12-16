using MURQ.URQL.Locations;

namespace MURQ.URQL.SyntaxTree.Statements;

public record PauseStatementSto(int Duration, Location Location) : StatementSto
{
    public override Location Location { get; } = Location;
}

using MURQ.URQL.Locations;

namespace MURQ.URQL.SyntaxTree.Statements;
public record PrintStatementSto(string Text, bool IsNewLineAtEnd, Location Location) : StatementSto
{
    public override Location Location { get; } = Location;
}

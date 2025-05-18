using MURQ.URQL.Locations;

namespace MURQ.URQL.SyntaxTree.Statements;

public abstract record StatementSto
{
    public required Location Location { get; init; }
}

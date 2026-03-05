using MURQ.Domain.Quests.Locations;

namespace MURQ.URQL.SyntaxTree;
public abstract record SyntaxTreeObject
{
    public abstract Location Location { get; }
}

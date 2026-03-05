using MURQ.Domain.Quests.Locations;

namespace MURQ.URQL.SyntaxTree.Expressions;

public record StringLiteralExpressionSto(string Text, Location Location) : ExpressionSto
{
    public override Location Location { get; } = Location;
}
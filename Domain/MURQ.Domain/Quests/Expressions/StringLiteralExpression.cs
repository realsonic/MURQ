using MURQ.Domain.Games;
using MURQ.Domain.Games.Values;

namespace MURQ.Domain.Quests.Expressions;

public class StringLiteralExpression : Expression
{
    public required string Text { get; init; }

    public override Value Calculate(IGameContext gameContext)
    {
        return new StringValue(Text);;
    }
}
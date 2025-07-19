using MURQ.Domain.Games;
using MURQ.Domain.Quests.Expressions;

namespace MURQ.Domain.Quests.UrqStrings;

public class UrqStringExpressionPart(Expression expression) : UrqStringPart
{
    public override string ToString(IGameContext gameContext)
    {
        var value = expression.Calculate(gameContext);
        return value.ToString();
    }
}
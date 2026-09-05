using MURQ.Domain.Games;

namespace MURQ.Domain.Quests.Expressions.Logic;

public abstract class LogicExpression
{
    public abstract bool Calculate(IGameContext gameContext);
}

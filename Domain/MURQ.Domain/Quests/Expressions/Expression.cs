using MURQ.Domain.Games;
using MURQ.Domain.Games.Values;

namespace MURQ.Domain.Quests.Expressions;

public abstract class Expression
{
    public abstract Value Calculate(IGameContext gameContext);
}

using MURQ.Domain.Games;

namespace MURQ.Domain.Quests.UrqStrings;

public abstract class UrqStringPart
{
    public abstract string ToString(IGameContext gameContext);
}

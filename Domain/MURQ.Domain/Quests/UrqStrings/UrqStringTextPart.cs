using MURQ.Domain.Games;

namespace MURQ.Domain.Quests.UrqStrings;

public class UrqStringTextPart(string text) : UrqStringPart
{
    public override string ToString(IGameContext gameContext) => text;
}

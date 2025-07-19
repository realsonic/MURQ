using MURQ.Domain.Quests.UrqStrings;

namespace MURQ.Application.UrqLoaders;
public class UrqStringLoader
{
    public static UrqString Load(string sourceString)
    {
        return new UrqString([new UrqStringTextPart(sourceString)]);
    }
}

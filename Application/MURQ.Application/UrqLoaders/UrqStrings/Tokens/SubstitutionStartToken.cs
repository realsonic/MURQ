using static MURQ.Application.UrqLoaders.UrqStrings.Tokens.SubstitutionStartToken;

namespace MURQ.Application.UrqLoaders.UrqStrings.Tokens;

public record SubstitutionStartToken(ModifierEnum SubstitutionModifier) : Token()
{
    public enum ModifierEnum
    {
        None,
        AsString
    }
}

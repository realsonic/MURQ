using MURQ.URQL.Locations;

using static MURQ.URQL.Substitutions.Tokens.SubstitutionStartToken;

namespace MURQ.URQL.Substitutions.Tokens;

public record SubstitutionStartToken(ModifierEnum SubstitutionModifier, Location Location) : Token(Location)
{
    public enum ModifierEnum
    {
        None,
        AsString
    }
}

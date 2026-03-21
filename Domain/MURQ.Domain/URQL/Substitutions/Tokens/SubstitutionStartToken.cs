using MURQ.Domain.URQL.Locations;

using static MURQ.Domain.URQL.Substitutions.Tokens.SubstitutionStartToken;

namespace MURQ.Domain.URQL.Substitutions.Tokens;

public record SubstitutionStartToken(ModifierEnum SubstitutionModifier, Location Location) : Token(Location)
{
    public enum ModifierEnum
    {
        None,
        AsString
    }
}

using static MURQ.URQL.Substitutions.Tokens.SubstitutionStartToken;

namespace MURQ.URQL.Substitutions.Tokens;

public record SubstitutionStartToken(ModifierEnum SubstitutionModifier) : Token()
{
    public enum ModifierEnum
    {
        None,
        AsString
    }
}

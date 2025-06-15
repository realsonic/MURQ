using MURQ.URQL.Locations;

using System.ComponentModel;

namespace MURQ.URQL.Tokens.If;

[Description("ветвление (then)")]
public record ThenToken(string Lexeme, Location Location) : Token(Lexeme, Location)
{
    public override string GetDescription() => "ветвление (then)";
}

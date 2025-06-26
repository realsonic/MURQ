using MURQ.URQL.Locations;

using System.ComponentModel;

namespace MURQ.URQL.Tokens.If;

[Description("ветвление (if)")]
public record IfToken(string Lexeme, Location Location) : Token(Lexeme, Location)
{
    public override string GetDescription() => "ветвление (if)";
}

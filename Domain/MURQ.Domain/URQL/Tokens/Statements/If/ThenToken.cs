using MURQ.Domain.URQL.Locations;

using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens.Statements.If;

[Description("ветвление (then)")]
public record ThenToken(string Lexeme, Location Location) : Token(Lexeme, Location)
{
    public override string GetDescription() => "ветвление (then)";
}

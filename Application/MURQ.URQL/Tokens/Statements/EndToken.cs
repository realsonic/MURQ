using MURQ.URQL.Locations;

using System.ComponentModel;

namespace MURQ.URQL.Tokens.Statements;

[Description("конец локации (end)")]
public record EndToken(string Lexeme, Location Location) : StatementToken(Lexeme, Location)
{
    public override string GetDescription() => "конец локации (end)";
}

using MURQ.URQL.Locations;

using System.ComponentModel;

namespace MURQ.URQL.Tokens.Statements;

[Description("безусловный переход (goto)")]
public record GotoToken(string Label, string Lexeme, Location Location) : StatementToken(Lexeme, Location)
{
    public override string GetDescription() => $"безусловный переход (goto), ведущий на метку {Label}";
}

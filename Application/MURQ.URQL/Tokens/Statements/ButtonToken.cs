using MURQ.URQL.Locations;

using System.ComponentModel;

namespace MURQ.URQL.Tokens.Statements;

[Description("кнопка (btn)")]
public record ButtonToken(string Label, string Caption, string Lexeme, Location Location) : StatementToken(Lexeme, Location)
{
    public override string GetDescription() => $"кнопка (btn) с надписью \"{Caption}\", ведущая на метку \"{Label}\"";
}

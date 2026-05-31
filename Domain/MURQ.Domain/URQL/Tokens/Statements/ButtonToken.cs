using MURQ.Domain.URQL.Locations;

using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens.Statements;

[Description("оператор кнопка (btn)")]
public record ButtonToken(string Label, string Caption, string Lexeme, Location Location) : StatementToken(Lexeme, Location)
{
    public override string Description => $@"оператор кнопка (btn) с надписью ""{Caption}"", ведущая на метку ""{Label}""";
}

using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens.Statements;

[Description("оператор кнопка (btn)")]
public record ButtonToken(string Label, string Caption, Lexeme Lexeme) : StatementToken(Lexeme)
{
    public override string Description => $@"оператор кнопка (btn) с надписью ""{Caption}"", ведущая на метку ""{Label}""";
}

using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens.Statements;

[Description("оператор безусловный переход (goto)")]
public record GotoToken(string Label, Lexeme Lexeme) : StatementToken(Lexeme)
{
    public override string Description => $@"оператор безусловный переход (goto), ведущий на метку ""{Label}""";
}

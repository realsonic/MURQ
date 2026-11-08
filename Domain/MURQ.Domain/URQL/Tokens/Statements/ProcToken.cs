using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens.Statements;

[Description("оператор процедура (proc)")]
public record ProcToken(string Label, Lexeme Lexeme) : StatementToken(Lexeme)
{
    public override string Description => $@"оператор процедура (proc), вызывающий процедуру по метке ""{Label}""";
}

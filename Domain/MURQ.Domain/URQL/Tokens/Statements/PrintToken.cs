using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens.Statements;

[Description("оператор печать (p/pln)")]
public record PrintToken(string Text, bool IsNewLineAtEnd, Lexeme Lexeme) : StatementToken(Lexeme)
{
    public override string Description => $@"оператор печать (p/pln) текста ""{Text}"" {(IsNewLineAtEnd ? "с новой строкой" : "без новой строки")}";
}

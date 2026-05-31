using MURQ.Domain.URQL.Locations;

using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens.Statements;

[Description("оператор печать (p/pln)")]
public record PrintToken(string Text, bool IsNewLineAtEnd, string Lexeme, Location Location) : StatementToken(Lexeme, Location)
{
    public override string Description => $@"оператор печать (p/pln) текста ""{Text}"" {(IsNewLineAtEnd ? "с новой строкой" : "без новой строки")}";
}

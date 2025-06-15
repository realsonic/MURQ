using MURQ.URQL.Locations;

using System.ComponentModel;

namespace MURQ.URQL.Tokens.Statements;

[Description("печать (p/pln)")]
public record PrintToken(string Text, bool IsNewLineAtEnd, string Lexeme, Location Location) : StatementToken(Lexeme, Location)
{
    public override string GetDescription()
    {
        return $"печать {(IsNewLineAtEnd ? "с новой строкой" : "без новой строки")} текста \"{Text}\"";
    }
}

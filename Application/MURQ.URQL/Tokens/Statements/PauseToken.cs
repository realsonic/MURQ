using MURQ.URQL.Locations;

using System.ComponentModel;

namespace MURQ.URQL.Tokens.Statements;

[Description("пауза (pause)")]
public record PauseToken(int Duration, string Lexeme, Location Location) : StatementToken(Lexeme, Location)
{
    public override string GetDescription() => $"паузе (pause) длительностью {Duration} мс";
}

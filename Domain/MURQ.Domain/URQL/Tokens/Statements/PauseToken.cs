using MURQ.Domain.URQL.Locations;

using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens.Statements;

[Description("оператор пауза (pause)")]
public record PauseToken(int Duration, string Lexeme, Location Location) : StatementToken(Lexeme, Location)
{
    public override string Description => $"оператор пауза (pause) длительностью {Duration} мс";
}

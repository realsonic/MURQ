using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens.Statements;

[Description("оператор пауза (pause)")]
public record PauseToken(int Duration, Lexeme Lexeme) : StatementToken(Lexeme)
{
    public override string Description => $"оператор пауза (pause) длительностью {Duration} мс";
}

using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens.Statements;

[Description("оператор конец локации (end)")]
public record EndToken(Lexeme Lexeme) : StatementToken(Lexeme);
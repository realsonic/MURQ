using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens.Statements;

[Description(@"объединение команд ""&""")]
public record StatementJoinToken(Lexeme Lexeme) : Token(Lexeme);
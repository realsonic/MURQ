using MURQ.Domain.URQL.Locations;

using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens.Statements;

[Description(@"объединение команд ""&""")]
public record StatementJoinToken(string Lexeme, Location Location) : Token(Lexeme, Location);
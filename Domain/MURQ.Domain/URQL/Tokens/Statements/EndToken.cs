using MURQ.Domain.URQL.Locations;

using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens.Statements;

[Description("оператор конец локации (end)")]
public record EndToken(string Lexeme, Location Location) : StatementToken(Lexeme, Location);
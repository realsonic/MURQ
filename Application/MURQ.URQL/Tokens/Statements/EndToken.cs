using MURQ.URQL.Locations;

namespace MURQ.URQL.Tokens.Statements;

public record EndToken(string Lexeme, Location Location) : StatementToken(Lexeme, Location);

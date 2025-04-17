using MURQ.URQL.Locations;

namespace MURQ.URQL.Tokens.Statements;
public record ClearScreenToken(string Lexeme, Location Location) : StatementToken(Lexeme, Location);
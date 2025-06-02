using MURQ.URQL.Locations;

namespace MURQ.URQL.Tokens.Statements.If;

public record ThenToken(string Lexeme, Location Location) : Token(Lexeme, Location);
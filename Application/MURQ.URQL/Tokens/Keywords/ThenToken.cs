using MURQ.URQL.Locations;

namespace MURQ.URQL.Tokens.Keywords;

public record ThenToken(string Lexeme, Location Location) : Token(Lexeme, Location);
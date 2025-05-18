using MURQ.URQL.Locations;

namespace MURQ.URQL.Tokens;

public record EqualityToken(string Lexeme, Location Location) : Token(Lexeme, Location);

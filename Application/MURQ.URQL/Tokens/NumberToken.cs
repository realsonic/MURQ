using MURQ.URQL.Locations;

namespace MURQ.URQL.Tokens;
public record NumberToken(decimal Value, string Lexeme, Location Location) : Token(Lexeme, Location);

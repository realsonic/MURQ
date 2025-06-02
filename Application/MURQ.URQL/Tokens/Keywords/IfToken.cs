using MURQ.URQL.Locations;

namespace MURQ.URQL.Tokens.Keywords;
public record IfToken(string Lexeme, Location Location) : Token(Lexeme, Location);
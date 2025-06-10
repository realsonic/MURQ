using MURQ.URQL.Locations;

namespace MURQ.URQL.Tokens.Statements.If;
public record IfToken(string Lexeme, Location Location) : Token(Lexeme, Location);
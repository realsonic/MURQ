using MURQ.URQL.Locations;

namespace MURQ.URQL.Tokens;
public record VariableToken(string Name, string Lexeme, Location Location) : Token(Lexeme, Location);

using MURQ.URQL.Locations;

namespace MURQ.URQL.Tokens;

public record NewLineToken(string Lexeme, Location Location) : Token(Lexeme, Location);

using MURQ.URQL.Tokens.Locations;

namespace MURQ.URQL.Tokens.Tokens;

public abstract record Token(string Lexeme, Location Location);
using MURQ.URQL.Locations;

namespace MURQ.URQL.Tokens;

public abstract record Token(string Lexeme, Location Location);
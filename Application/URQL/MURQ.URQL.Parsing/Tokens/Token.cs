using MURQ.URQL.Parsing.Locations;

namespace MURQ.URQL.Parsing.Tokens;

public abstract record Token(string Lexeme, Location Location);
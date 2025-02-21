using MURQ.URQL.Parsing.Locations;
using MURQ.URQL.Parsing.Tokens;

namespace MURQ.URQL.Parsing.Tokens.Statements;
public abstract record StatementToken(string Lexeme, Location Location) : Token(Lexeme, Location);
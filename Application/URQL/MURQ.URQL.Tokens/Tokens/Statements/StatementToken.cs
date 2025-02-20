using MURQ.URQL.Tokens.Locations;

namespace MURQ.URQL.Tokens.Tokens.Statements;
public abstract record StatementToken(string Lexeme, Location Location) : Token(Lexeme, Location);
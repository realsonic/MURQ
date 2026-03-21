using MURQ.Domain.URQL.Locations;

namespace MURQ.URQL.Tokens.Statements;
public abstract record StatementToken(string Lexeme, Location Location) : Token(Lexeme, Location);
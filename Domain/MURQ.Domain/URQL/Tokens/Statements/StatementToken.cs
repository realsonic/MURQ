using MURQ.Domain.URQL.Locations;

namespace MURQ.Domain.URQL.Tokens.Statements;

public abstract record StatementToken(string Lexeme, Location Location) : Token(Lexeme, Location);
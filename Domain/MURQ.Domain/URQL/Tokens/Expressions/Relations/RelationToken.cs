using MURQ.Domain.URQL.Locations;

namespace MURQ.Domain.URQL.Tokens.Expressions.Relations;

public abstract record RelationToken(string Lexeme, Location Location) : Token(Lexeme, Location);
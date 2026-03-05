using MURQ.Domain.Quests.Locations;

namespace MURQ.URQL.Tokens.Statements;
public abstract record StatementToken(string Lexeme, Location Location) : Token(Lexeme, Location);
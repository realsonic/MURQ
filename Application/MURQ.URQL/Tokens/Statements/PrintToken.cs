using MURQ.URQL.Locations;

namespace MURQ.URQL.Tokens.Statements;
public record PrintToken(string Text, string Lexeme, Location Location) : StatementToken(Lexeme, Location);

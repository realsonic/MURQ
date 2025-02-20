using MURQ.URQL.Tokens.Locations;

namespace MURQ.URQL.Tokens.Tokens.Statements;
public record PrintToken(string Text, string Lexeme, Location Location) : StatementToken(Lexeme, Location);

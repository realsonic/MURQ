using MURQ.URQL.Parsing.Locations;

namespace MURQ.URQL.Parsing.Tokens.Statements;
public record PrintToken(string Text, string Lexeme, Location Location) : StatementToken(Lexeme, Location);

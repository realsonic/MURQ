using MURQ.URQL.Locations;

namespace MURQ.URQL.Tokens.Statements;
public record PrintToken(string Text, bool IsNewLineAtEnd, string Lexeme, Location Location) : StatementToken(Lexeme, Location);

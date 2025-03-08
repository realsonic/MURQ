using MURQ.URQL.Locations;

namespace MURQ.URQL.Tokens.Statements;

public record ButtonToken(string Label, string Caption, string Lexeme, Location Location) : StatementToken(Lexeme, Location);

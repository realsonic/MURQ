using MURQ.URQL.Locations;

namespace MURQ.URQL.Tokens.Statements;

public record LabelToken(string Label, string Lexeme, Location Location) : StatementToken(Lexeme, Location);

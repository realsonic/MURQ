using MURQ.Domain.URQL.Locations;

using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens.Statements.If;

[Description("элемент if оператора ветвления if-then-else")]
public record IfToken(string Lexeme, Location Location) : Token(Lexeme, Location);
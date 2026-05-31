using MURQ.Domain.URQL.Locations;

using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens.Statements.If;

[Description("элемент then оператора ветвления if-then-else")]
public record ThenToken(string Lexeme, Location Location) : Token(Lexeme, Location);
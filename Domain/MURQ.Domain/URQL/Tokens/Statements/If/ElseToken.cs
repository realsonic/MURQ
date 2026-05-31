using MURQ.Domain.URQL.Locations;

using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens.Statements.If;

[Description("элемент else оператора ветвления if-then-else")]
public record ElseToken(string Lexeme, Location Location) : Token(Lexeme, Location);
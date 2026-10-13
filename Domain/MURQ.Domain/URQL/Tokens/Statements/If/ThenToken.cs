using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens.Statements.If;

[Description("элемент then оператора ветвления if-then-else")]
public record ThenToken(Lexeme Lexeme) : Token(Lexeme);
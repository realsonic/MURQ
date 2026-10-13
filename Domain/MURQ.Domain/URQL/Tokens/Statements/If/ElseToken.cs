using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens.Statements.If;

[Description("элемент else оператора ветвления if-then-else")]
public record ElseToken(Lexeme Lexeme) : Token(Lexeme);
using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens.Statements.If;

[Description("элемент if оператора ветвления if-then-else")]
public record IfToken(Lexeme Lexeme) : Token(Lexeme);
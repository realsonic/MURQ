using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens.Expressions.Logic;

[Description(@"логическое ИЛИ ""or""")]
public record OrToken(Lexeme Lexeme) : Token(Lexeme);
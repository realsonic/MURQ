using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens.Expressions;

[Description(@"умножение ""*""")]
public record MultiplicationToken(Lexeme Lexeme) : Token(Lexeme);

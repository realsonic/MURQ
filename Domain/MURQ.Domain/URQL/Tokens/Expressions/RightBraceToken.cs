using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens.Expressions;

[Description(@"правая скобка "")""")]
public record RightBraceToken(Lexeme Lexeme) : Token(Lexeme);
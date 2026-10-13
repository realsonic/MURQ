using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens.Expressions;

[Description(@"минус ""-""")]
public record MinusToken(Lexeme Lexeme) : Token(Lexeme);
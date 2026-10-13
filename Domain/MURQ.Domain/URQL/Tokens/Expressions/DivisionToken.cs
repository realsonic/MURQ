using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens.Expressions;

[Description(@"деление ""/""")]
public record DivisionToken(Lexeme Lexeme) : Token(Lexeme);
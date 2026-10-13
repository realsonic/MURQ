using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens.Expressions;

[Description(@"плюс ""+""")]
public record PlusToken(Lexeme Lexeme) : Token(Lexeme);

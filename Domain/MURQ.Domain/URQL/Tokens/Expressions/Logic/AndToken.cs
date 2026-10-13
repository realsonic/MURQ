using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens.Expressions.Logic;

[Description(@"логическое И ""and""")]
public record AndToken(Lexeme Lexeme) : Token(Lexeme);
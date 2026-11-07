using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens.Expressions.Logic;

[Description(@"логическое НЕ ""not""")]
public record NotToken(Lexeme Lexeme) : Token(Lexeme);
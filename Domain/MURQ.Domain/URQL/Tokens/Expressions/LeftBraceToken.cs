using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens.Expressions;

[Description(@"левая скобка ""(""")]
public record LeftBraceToken(Lexeme Lexeme) : Token(Lexeme);
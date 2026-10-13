using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens;

[Description(@"равно ""=""")]
public record EqualityToken(Lexeme Lexeme) : Token(Lexeme);
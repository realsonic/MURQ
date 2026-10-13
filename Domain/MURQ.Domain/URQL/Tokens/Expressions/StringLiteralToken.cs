using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens.Expressions;

[Description("строковый литерал")]
public record StringLiteralToken(string Text, Lexeme Lexeme) : Token(Lexeme)
{
    public override string Description => $@"строковый литерал ""{Text}""";
}

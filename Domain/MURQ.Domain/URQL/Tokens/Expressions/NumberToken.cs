using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens.Expressions;

[Description("число")]
public record NumberToken(decimal Value, Lexeme Lexeme) : Token(Lexeme)
{
    public override string Description => $"число {Value}";
}

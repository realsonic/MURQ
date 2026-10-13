using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens.Expressions;

[Description("переменная")]
public record VariableToken(string Name, Lexeme Lexeme) : Token(Lexeme)
{
    public override string Description => $@"переменная ""{Name}""";
}

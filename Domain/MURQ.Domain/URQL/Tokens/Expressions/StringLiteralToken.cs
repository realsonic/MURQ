using MURQ.Domain.URQL.Locations;

using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens.Expressions;

[Description("строковый литерал")]
public record StringLiteralToken(string Text, string Lexeme, Location Location) : Token(Lexeme, Location)
{
    public override string Description => $@"строковый литерал ""{Text}""";
}

using MURQ.Domain.URQL.Locations;

using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens;

[Description("строковый литерал")]
public record StringLiteralToken(string Text, string Lexeme, Location Location) : Token(Lexeme, Location)
{
    public override string GetDescription() => $"строковый литерал \"{Text}\"";
}

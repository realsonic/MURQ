using MURQ.URQL.Locations;

using System.ComponentModel;

namespace MURQ.URQL.Tokens;

[Description("число")]
public record NumberToken(decimal Value, string Lexeme, Location Location) : Token(Lexeme, Location)
{
    public override string GetDescription() => $"число {Value}";
}

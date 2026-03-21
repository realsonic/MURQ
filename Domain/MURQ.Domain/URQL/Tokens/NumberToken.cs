using MURQ.Domain.URQL.Locations;

using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens;

[Description("число")]
public record NumberToken(decimal Value, string Lexeme, Location Location) : Token(Lexeme, Location)
{
    public override string GetDescription() => $"число {Value}";
}

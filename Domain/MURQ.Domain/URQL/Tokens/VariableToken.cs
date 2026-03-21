using MURQ.Domain.URQL.Locations;

using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens;

[Description("переменная")]
public record VariableToken(string Name, string Lexeme, Location Location) : Token(Lexeme, Location)
{
    public override string GetDescription() => $"переменная {Name}";
}

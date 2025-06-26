using MURQ.URQL.Locations;

using System.ComponentModel;

namespace MURQ.URQL.Tokens;

[Description("переменная")]
public record VariableToken(string Name, string Lexeme, Location Location) : Token(Lexeme, Location)
{
    public override string GetDescription() => $"переменная {Name}";
}

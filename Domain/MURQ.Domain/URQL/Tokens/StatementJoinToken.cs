using MURQ.Domain.URQL.Locations;

using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens;

[Description("объединение команд (&)")]
public record StatementJoinToken(string Lexeme, Location Location) : Token(Lexeme, Location)
{
    public override string GetDescription()
    {
        return "объединение команд (&)";
    }
}
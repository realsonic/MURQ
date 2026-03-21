using MURQ.Domain.URQL.Locations;

using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens.Statements;

[Description("очистка экрана (cls)")]
public record ClearScreenToken(string Lexeme, Location Location) : StatementToken(Lexeme, Location)
{
    public override string GetDescription() => "очистка экрана (cls)";
}

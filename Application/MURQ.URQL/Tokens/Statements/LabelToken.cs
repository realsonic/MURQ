using MURQ.URQL.Locations;

using System.ComponentModel;

namespace MURQ.URQL.Tokens.Statements;

[Description("метка")]
public record LabelToken(string Label, string Lexeme, Location Location) : StatementToken(Lexeme, Location)
{
    public override string GetDescription() => $"метка \"{Label}\"";
}

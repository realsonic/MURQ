using MURQ.Domain.URQL.Locations;

using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens.Statements;

[Description("оператор безусловный переход (goto)")]
public record GotoToken(string Label, string Lexeme, Location Location) : StatementToken(Lexeme, Location)
{
    public override string Description => $@"оператор безусловный переход (goto), ведущий на метку ""{Label}""";
}

using MURQ.Domain.URQL.Locations;

using System.ComponentModel;

namespace MURQ.Domain.URQL.Tokens;

[Description("новая строка")]
public record NewLineToken(string Lexeme, Location Location) : Token(Lexeme, Location)
{
    public NewLineToken(char character, Position position) : this(character.ToString(), Location.StartAt(position)) { }

    public override string GetDescription() => "новая строка";
}

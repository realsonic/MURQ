using MURQ.URQL.Locations;

using System.ComponentModel;

namespace MURQ.URQL.Tokens;

[Description("равно (=)")]
public record EqualityToken(string Lexeme, Location Location) : Token(Lexeme, Location)
{
    public EqualityToken(char character, Position position) : this(character.ToString(), Location.StartAt(position)) { }

    public override string GetDescription() => "равно (=)";
}

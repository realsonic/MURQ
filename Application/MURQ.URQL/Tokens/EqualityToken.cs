using MURQ.URQL.Locations;

namespace MURQ.URQL.Tokens;

public record EqualityToken(string Lexeme, Location Location) : Token(Lexeme, Location)
{
    public EqualityToken(char character, Position position) : this(character.ToString(), Location.StartAt(position)) { }
}

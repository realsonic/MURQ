using MURQ.URQL.Locations;

namespace MURQ.URQL.Tokens;

public record NewLineToken(string Lexeme, Location Location) : Token(Lexeme, Location)
{
    public NewLineToken(char character, Position position) : this(character.ToString(), Location.StartAt(position)) { }
}

using MURQ.URQL.Locations;
using MURQ.URQL.Tokens;

namespace MURQ.URQL.Lexing.Monads.URQL;
public record NumberMonad(string Lexeme, Location Location) : UncompletedLexemeMonad(Lexeme, Location)
{
    public static NumberMonad Start(char startCharacter, Position startPosition) => new(startCharacter.ToString(), Location.StartAt(startPosition));

    public override LexemeMonad Append(char character, Position position) => character switch
    {
        _ when char.IsNumber(character) => Proceed(character, position),
        _ => CreateTokenMonad((character, position))
    };

    public override LexemeMonad Finalize() => CreateTokenMonad();

    private LexemeMonad CreateTokenMonad((char Character, Position Position)? remain = null)
    {
        if (decimal.TryParse(Lexeme, out decimal value))
        {
            NumberToken numberToken = new(value, Lexeme, Location);

            return remain is not null
                ? numberToken.AsMonadWithRemain(remain.Value.Character, remain.Value.Position)
                : numberToken.AsMonad();
        }

        return new UnknownLexemeMonad(Lexeme, Location, $"{Lexeme} не подходит как число.");
    }
}
using MURQ.URQL.Locations;
using MURQ.URQL.Tokens;

namespace MURQ.URQL.Lexing.Monads.URQL;
public record NumberMonad(string Lexeme, Location Location) : UncompletedLexemeMonad(Lexeme, Location)
{
    public static NumberMonad Start(char startCharacter, Position startPosition) => new(startCharacter.ToString(), Location.StartAt(startPosition));

    public override LexemeMonad Append(char character, Position position) => character switch
    {
        _ when char.IsNumber(character) => Proceed(character, position),
        _ => new CompletedLexemeMonad(new NumberToken(decimal.Parse(Lexeme), Lexeme, Location), RootMonad.Remain(character, position))
    };

    public override LexemeMonad Finalize() => new CompletedLexemeMonad(new NumberToken(decimal.Parse(Lexeme), Lexeme, Location), null);
}
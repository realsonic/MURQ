using MURQ.URQL.Locations;

namespace MURQ.URQL.Lexing.Monads.URQL;

public record CommentMonad(string Lexeme, Location Location) : UncompletedLexemeMonad(Lexeme, Location)
{
    public static CommentMonad Start(char startCharacter, Position startPosition) => new(startCharacter.ToString(), Location.StartAt(startPosition));

    public override LexemeMonad Append(char character, Position position) => character switch
    {
        '\n' => RootMonad.Remain(character, position),
        _ => Proceed(character, position)
    };

    public override LexemeMonad Finalize() => new RootMonad(Location.End);
}
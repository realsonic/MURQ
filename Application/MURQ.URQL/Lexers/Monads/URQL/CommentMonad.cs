using MURQ.URQL.Locations;

namespace MURQ.URQL.Lexers.Monads.URQL;

public record CommentMonad(string Lexeme, Location Location) : UncompletedLexemeMonad(Lexeme, Location)
{
    public override LexemeMonad Append(char character, Position position) => character switch
    {
        '\n' => RootMonad.Remain(character, position),
        _ => Proceed(character, position)
    };

    public override LexemeMonad Finalize() => new RootMonad(Location.End);
}
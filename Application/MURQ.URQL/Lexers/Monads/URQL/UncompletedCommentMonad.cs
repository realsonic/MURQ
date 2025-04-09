using MURQ.URQL.Locations;

namespace MURQ.URQL.Lexers.Monads.URQL;

public record UncompletedCommentMonad(string Lexeme, Location Location) : UncompletedLexemeMonad(Lexeme, Location)
{
    public override LexemeMonad Append(char character, Position position) => character switch
    {
        '\n' => RootMonad.Remain(character, position),
        _ => new UncompletedCommentMonad(Lexeme + character, Location.EndAt(position))
    };

    public override LexemeMonad Finalize() => new RootMonad(Location.End);
}
using MURQ.URQL.Locations;
using MURQ.URQL.Tokens.Statements;

using static MURQ.URQL.Lexers.Monads.URQL.MaybeEndMonad;

namespace MURQ.URQL.Lexers.Monads.URQL;

public record MaybeEndMonad(EndLexemeProgress LexemeProgress, string Lexeme, Location Location) : UncompletedLexemeMonad(Lexeme, Location)
{
    public override LexemeMonad Append(char character, Position position) => LexemeProgress switch
    {
        EndLexemeProgress.E when character is 'n' or 'N' => new MaybeEndMonad(EndLexemeProgress.EN, Lexeme + character, Location.EndAt(position)),
        EndLexemeProgress.EN when character is 'd' or 'D' => new MaybeEndMonad(EndLexemeProgress.END, Lexeme + character, Location.EndAt(position)),
        EndLexemeProgress.END when character is ' ' or '\t' or '\n' => new CompletedLexemeMonad(new EndToken(Lexeme, Location), RootMonad.Remain(character, position)),
        _ => new UnknownLexemeMonad(Lexeme + character, Location.EndAt(position))
    };

    public override LexemeMonad Finalize() => LexemeProgress switch
    {
        EndLexemeProgress.END => new CompletedLexemeMonad(new EndToken(Lexeme, Location), null),
        _ => new UnknownLexemeMonad(Lexeme, Location),
    };

    public enum EndLexemeProgress
    {
        E,
        EN,
        END
    }
}

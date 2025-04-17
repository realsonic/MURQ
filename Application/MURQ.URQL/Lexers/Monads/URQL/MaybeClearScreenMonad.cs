using MURQ.URQL.Locations;
using MURQ.URQL.Tokens.Statements;

using static MURQ.URQL.Lexers.Monads.URQL.MaybeClearScreenMonad;

namespace MURQ.URQL.Lexers.Monads.URQL;
public record MaybeClearScreenMonad(ClearScreenLexemeProgress LexemeProgress, string Lexeme, Location Location) : UncompletedLexemeMonad(Lexeme, Location)
{
    public override LexemeMonad Append(char character, Position position) => LexemeProgress switch
    {
        ClearScreenLexemeProgress.C when character is 'l' or 'L' => new MaybeClearScreenMonad(ClearScreenLexemeProgress.CL, Lexeme + character, Location.EndAt(position)),
        ClearScreenLexemeProgress.CL when character is 's' or 'S' => new MaybeClearScreenMonad(ClearScreenLexemeProgress.CLS, Lexeme + character, Location.EndAt(position)),
        ClearScreenLexemeProgress.CLS when character is ' ' or '\t' or '\n' or ';' => new CompletedLexemeMonad(new ClearScreenToken(Lexeme, Location), RootMonad.Remain(character, position)),
        _ => new UnknownLexemeMonad(Lexeme + character, Location.EndAt(position))
    };

    public override LexemeMonad Finalize() => LexemeProgress switch
    {
        ClearScreenLexemeProgress.CLS => new CompletedLexemeMonad(new ClearScreenToken(Lexeme, Location), null),
        _ => new UnknownLexemeMonad(Lexeme, Location)
    };

    public enum ClearScreenLexemeProgress
    {
        C,
        CL,
        CLS
    }
}

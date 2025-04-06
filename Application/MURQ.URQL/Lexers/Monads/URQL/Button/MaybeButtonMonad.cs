using MURQ.URQL.Locations;

using static MURQ.URQL.Lexers.Monads.URQL.Button.MaybeButtonMonad;
using static MURQ.URQL.Lexers.Monads.URQL.Button.UncompletedButtonMonad;

namespace MURQ.URQL.Lexers.Monads.URQL.Button;

public record MaybeButtonMonad(MaybeButtonLexemeProgress LexemeProgress, string Lexeme, Location Location) : UncompletedLexemeMonad(Lexeme, Location)
{
    public override LexemeMonad Append(char character, Position position) => LexemeProgress switch
    {
        MaybeButtonLexemeProgress.B when character is 't' or 'T' => new MaybeButtonMonad(MaybeButtonLexemeProgress.BT, Lexeme + character, Location.EndAt(position)),
        MaybeButtonLexemeProgress.BT when character is 'n' or 'N' => new MaybeButtonMonad(MaybeButtonLexemeProgress.BTN, Lexeme + character, Location.EndAt(position)),
        MaybeButtonLexemeProgress.BTN when character is ' ' or '\t' => new UncompletedButtonMonad(UncompletedButtonLexemeProgress.Label, string.Empty, string.Empty, Lexeme + character, Location.EndAt(position)),
        _ => new UnknownLexemeMonad(Lexeme + character, Location.EndAt(position))
    };

    public override LexemeMonad Finalize() => new UnknownLexemeMonad(Lexeme, Location);

    public enum MaybeButtonLexemeProgress
    {
        B,
        BT,
        BTN
    }
}

using MURQ.URQL.Locations;

using static MURQ.URQL.Lexers.Monads.URQL.Button.MaybeButtonMonad;

namespace MURQ.URQL.Lexers.Monads.URQL.Button;

public record MaybeButtonMonad(ButtonLexemeProgress LexemeProgress, string Lexeme, Location Location) : UncompletedLexemeMonad(Lexeme, Location)
{
    public override LexemeMonad Append(char character, Position position) => LexemeProgress switch
    {
        ButtonLexemeProgress.B when character is 't' or 'T' => new MaybeButtonMonad(ButtonLexemeProgress.BT, Lexeme + character, Location.EndAt(position)),
        ButtonLexemeProgress.BT when character is 'n' or 'N' => new MaybeButtonMonad(ButtonLexemeProgress.BTN, Lexeme + character, Location.EndAt(position)),
        ButtonLexemeProgress.BTN when character is ' ' => new UncompletedButtonMonad(false, string.Empty, string.Empty, Lexeme + character, Location.EndAt(position)),
        _ => new UnknownLexemeMonad(Lexeme + character, Location.EndAt(position))
    };

    public override LexemeMonad Finalize() => new UnknownLexemeMonad(Lexeme, Location);

    public enum ButtonLexemeProgress
    {
        B,
        BT,
        BTN
    }
}

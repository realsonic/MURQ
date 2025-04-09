using MURQ.URQL.Locations;
using MURQ.URQL.Tokens.Statements;

using static MURQ.URQL.Lexers.Monads.URQL.Button.UncompletedButtonMonad;

namespace MURQ.URQL.Lexers.Monads.URQL.Button;

public record UncompletedButtonMonad(UncompletedButtonLexemeProgress LexemeProgress, string Label, string Caption, string Lexeme, Location Location) : UncompletedLexemeMonad(Lexeme, Location)
{
    public override LexemeMonad Append(char character, Position position)
    {
        return LexemeProgress switch
        {
            UncompletedButtonLexemeProgress.Label => character switch
            {
                ',' => new UncompletedButtonMonad(UncompletedButtonLexemeProgress.Caption, Label, string.Empty, Lexeme + character, Location.EndAt(position)),
                not '\n' => new UncompletedButtonMonad(UncompletedButtonLexemeProgress.Label, Label + character, string.Empty, Lexeme + character, Location.EndAt(position)),
                _ => new UnknownLexemeMonad(Lexeme + character, Location.EndAt(position))
            },

            UncompletedButtonLexemeProgress.Caption => character switch
            {
                '\n' or ';' => new CompletedLexemeMonad(new ButtonToken(Label, Caption, Lexeme, Location), RootMonad.Remain(character, position)),
                _ => new UncompletedButtonMonad(UncompletedButtonLexemeProgress.Caption, Label, Caption + character, Lexeme + character, Location.EndAt(position))
            },

            _ => throw new LexingException($"Неожиданное состояние монады лексемы кнопки: {nameof(LexemeProgress)} = {LexemeProgress}")
        };
    }

    public override LexemeMonad Finalize() => LexemeProgress is UncompletedButtonLexemeProgress.Caption
            ? new CompletedLexemeMonad(new ButtonToken(Label, Caption, Lexeme, Location), null)
            : new UnknownLexemeMonad(Lexeme, Location);

    public enum UncompletedButtonLexemeProgress
    {
        Label,
        Caption
    }
}

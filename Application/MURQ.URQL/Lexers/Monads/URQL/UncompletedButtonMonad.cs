using MURQ.URQL.Locations;
using MURQ.URQL.Tokens.Statements;

using static MURQ.URQL.Lexers.Monads.URQL.UncompletedButtonMonad;

namespace MURQ.URQL.Lexers.Monads.URQL;

public record UncompletedButtonMonad(ButtonLexemeProgress LexemeProgress, string Label, string Caption, string Lexeme, Location Location) : UncompletedLexemeMonad(Lexeme, Location)
{
    public static UncompletedButtonMonad StartAfterBtn(string lexeme, Location location) 
        => new(ButtonLexemeProgress.JustAfterBtn, string.Empty, string.Empty, lexeme, location);

    public override LexemeMonad Append(char character, Position position) => LexemeProgress switch
    {
        ButtonLexemeProgress.JustAfterBtn => character switch
        {
            ' ' or '\t' => new UncompletedButtonMonad(ButtonLexemeProgress.Label, string.Empty, string.Empty, Lexeme + character, Location.EndAt(position)),
            _ => new UnknownLexemeMonad(Lexeme + character, Location.EndAt(position), WhitespaceExpected)
        },

        ButtonLexemeProgress.Label => character switch
        {
            ',' => new UncompletedButtonMonad(ButtonLexemeProgress.Caption, Label, string.Empty, Lexeme + character, Location.EndAt(position)),
            not '\n' => new UncompletedButtonMonad(ButtonLexemeProgress.Label, Label + character, string.Empty, Lexeme + character, Location.EndAt(position)),
            _ => new UnknownLexemeMonad(Lexeme + character, Location.EndAt(position), CommaExpected)
        },

        ButtonLexemeProgress.Caption => character switch
        {
            '\n' or ';' => new CompletedLexemeMonad(new ButtonToken(Label, Caption, Lexeme, Location), RootMonad.Remain(character, position)),
            _ => new UncompletedButtonMonad(ButtonLexemeProgress.Caption, Label, Caption + character, Lexeme + character, Location.EndAt(position))
        },

        _ => throw new LexingException($"Неожиданное состояние монады лексемы кнопки: {nameof(LexemeProgress)} = {LexemeProgress}")
    };

    public override LexemeMonad Finalize() => LexemeProgress switch
    {
        ButtonLexemeProgress.JustAfterBtn => new UnknownLexemeMonad(Lexeme, Location, WhitespaceExpected),
        ButtonLexemeProgress.Label => new UnknownLexemeMonad(Lexeme, Location, CommaExpected),
        ButtonLexemeProgress.Caption => new CompletedLexemeMonad(new ButtonToken(Label, Caption, Lexeme, Location), null),
        _ => throw new LexingException($"Неожиданное состояние монады лексемы кнопки: {nameof(LexemeProgress)} = {LexemeProgress}")
    };

    public enum ButtonLexemeProgress
    {
        JustAfterBtn,
        Label,
        Caption
    }

    private const string WhitespaceExpected = "После btn ожидался пробел или табуляция";
    private const string CommaExpected = "В команде btn после метки ожидалась запятая";
}

using MURQ.URQL.Locations;
using MURQ.URQL.Tokens.Statements;

using static MURQ.URQL.Lexing.Monads.URQL.Statements.GotoMonad;

namespace MURQ.URQL.Lexing.Monads.URQL.Statements;
public record GotoMonad(GotoLexemeProgress LexemeProgress, string Label, string Lexeme, Location Location) : UncompletedLexemeMonad(Lexeme, Location)
{
    public static GotoMonad StartAfterGoto(string Lexeme, Location Location)
        => new(GotoLexemeProgress.JustAfterGoto, string.Empty, Lexeme, Location);

    public override LexemeMonad Append(char character, Position position) => LexemeProgress switch
    {
        GotoLexemeProgress.JustAfterGoto => character switch
        {
            ' ' or '\t' => new GotoMonad(GotoLexemeProgress.Label, string.Empty, Lexeme + character, Location.EndAt(position)),
            _ => new UnknownLexemeMonad(Lexeme + character, Location.EndAt(position), WhitespaceExpected)
        },

        GotoLexemeProgress.Label => character switch
        {
            '\n' => new GotoToken(Label, Lexeme, Location).AsMonadWithRemain(character, position),
            _ => new GotoMonad(GotoLexemeProgress.Label, Label + character, Lexeme + character, Location.EndAt(position))
        },

        _ => throw new LexingException($"Неожиданное состояние монады лексемы безусловного перехода: {nameof(LexemeProgress)} = {LexemeProgress}")
    };

    public override LexemeMonad Finalize() => LexemeProgress switch
    {
        GotoLexemeProgress.JustAfterGoto => new UnknownLexemeMonad(Lexeme, Location, WhitespaceExpected),
        GotoLexemeProgress.Label => new GotoToken(Label, Lexeme, Location).AsMonad(),
        _ => throw new LexingException($"Неожиданное состояние монады лексемы безусловного перехода: {nameof(LexemeProgress)} = {LexemeProgress}")
    };

    public enum GotoLexemeProgress
    {
        JustAfterGoto,
        Label
    }

    private const string WhitespaceExpected = "После goto ожидался пробел или табуляция";
}

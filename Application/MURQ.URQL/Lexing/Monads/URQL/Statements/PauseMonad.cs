using MURQ.URQL.Locations;
using MURQ.URQL.Tokens.Statements;

using static MURQ.URQL.Lexing.Monads.URQL.Statements.PauseMonad;

namespace MURQ.URQL.Lexing.Monads.URQL.Statements;

public record PauseMonad(PauseLexemeProgress LexemeProgress, string DurationLexeme, string Lexeme, Location Location) : UncompletedLexemeMonad(Lexeme, Location)
{
    public static PauseMonad StartAfterPause(string lexeme, Location location)
        => new(PauseLexemeProgress.JustAfterPause, string.Empty, lexeme, location);

    public override LexemeMonad Append(char character, Position position) => LexemeProgress switch
    {
        PauseLexemeProgress.JustAfterPause => character switch
        {
            ' ' or '\t' => new PauseMonad(PauseLexemeProgress.Duration, string.Empty, Lexeme + character, Location.EndAt(position)),
            _ => new UnknownLexemeMonad(Lexeme + character, Location.EndAt(position), WhitespaceExpected)
        },

        PauseLexemeProgress.Duration => character switch
        {
            '\n' => CreateTokenMonad((character, position)),
            _ when char.IsNumber(character) => new PauseMonad(PauseLexemeProgress.Duration, DurationLexeme + character, Lexeme + character, Location.EndAt(position)),
            _ => new UnknownLexemeMonad(Lexeme + character, Location.EndAt(position), NumberExpected)
        },

        _ => throw new LexingException($"Неожиданное состояние монады лексемы паузы: {nameof(LexemeProgress)} = {LexemeProgress}")
    };

    public override LexemeMonad Finalize() => LexemeProgress switch
    {
        PauseLexemeProgress.JustAfterPause => new UnknownLexemeMonad(Lexeme, Location, WhitespaceExpected),
        PauseLexemeProgress.Duration => CreateTokenMonad(),
        _ => throw new LexingException($"Неожиданное состояние монады лексемы паузы: {nameof(LexemeProgress)} = {LexemeProgress}")
    };

    private LexemeMonad CreateTokenMonad((char Character, Position Position)? remain = null)
    {
        if (int.TryParse(DurationLexeme, out int duration))
        {
            PauseToken pauseToken = new(duration, Lexeme, Location);

            return remain is not null
                ? pauseToken.AsMonadWithRemain(remain.Value.Character, remain.Value.Position)
                : pauseToken.AsMonad();
        }

        return new UnknownLexemeMonad(Lexeme, Location, $"{DurationLexeme} не подходит как число.");
    }

    public enum PauseLexemeProgress
    {
        JustAfterPause,
        Duration
    }

    private const string WhitespaceExpected = "После pause ожидался пробел или табуляция";
    private const string NumberExpected = "В команде pause после пробельного символа ожидалось число";
}

using MURQ.URQL.Locations;
using MURQ.URQL.Tokens.Statements;

using static MURQ.URQL.Lexers.Monads.URQL.Print.MaybePrintMonad;

namespace MURQ.URQL.Lexers.Monads.URQL.Print;

public record MaybePrintMonad(PrintLexemeProgress LexemeProgress, string Lexeme, Location Location) : UncompletedLexemeMonad(Lexeme, Location)
{
    public override LexemeMonad Append(char character, Position position) => LexemeProgress switch
    {
        PrintLexemeProgress.P => character switch
        {
            'l' or 'L' => new MaybePrintMonad(PrintLexemeProgress.PL, Lexeme + character, Location.EndAt(position)),
            ' ' or '\t' => new UncompletedPrintMonad(string.Empty, false, Lexeme + character, Location.EndAt(position)),
            '\n' or ';' => new CompletedLexemeMonad(new PrintToken(string.Empty, false, Lexeme, Location), RootMonad.Remain(character, position)),
            _ => new UnknownLexemeMonad(Lexeme + character, Location.EndAt(position))
        },

        PrintLexemeProgress.PL => character switch
        {
            'n' or 'N' => new MaybePrintMonad(PrintLexemeProgress.PLN, Lexeme + character, Location.EndAt(position)),
            _ => new UnknownLexemeMonad(Lexeme + character, Location.EndAt(position))
        },

        PrintLexemeProgress.PLN => character switch
        {
            ' ' or '\t' => new UncompletedPrintMonad(string.Empty, true, Lexeme + character, Location.EndAt(position)),
            '\n' or ';' => new CompletedLexemeMonad(new PrintToken(string.Empty, true, Lexeme, Location), RootMonad.Remain(character, position)),
            _ => new UnknownLexemeMonad(Lexeme + character, Location.EndAt(position))
        },

        _ => throw new LexingException($"Неожиданное состояние монады лексемы печати: {nameof(LexemeProgress)} = {LexemeProgress}")
    };

    public override LexemeMonad Finalize() => LexemeProgress switch
    {
        PrintLexemeProgress.P => new CompletedLexemeMonad(new PrintToken(string.Empty, false, Lexeme, Location), null),
        PrintLexemeProgress.PLN => new CompletedLexemeMonad(new PrintToken(string.Empty, true, Lexeme, Location), null),
        _ => new UnknownLexemeMonad(Lexeme, Location),
    };

    public enum PrintLexemeProgress
    {
        P,
        PL,
        PLN
    }
}

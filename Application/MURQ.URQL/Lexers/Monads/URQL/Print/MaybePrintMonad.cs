using MURQ.URQL.Locations;
using MURQ.URQL.Tokens.Statements;

using static MURQ.URQL.Lexers.Monads.URQL.Print.MaybePrintMonad;

namespace MURQ.URQL.Lexers.Monads.URQL.Print;

public record MaybePrintMonad(PrintLexemeProgress LexemeProgress, string Lexeme, Location Location) : UncompletedLexemeMonad(Lexeme, Location)
{
    public override LexemeMonad Append(char character, Position position)
    {
        switch (LexemeProgress)
        {
            case PrintLexemeProgress.P:
                switch (character)
                {
                    case ' ': return new UncompletedPrintMonad(string.Empty, false, Lexeme + character, Location.EndAt(position));
                    case '\n': return new CompletedLexemeMonad(new PrintToken(string.Empty, false, Lexeme, Location), null);
                    case 'l' or 'L': return new MaybePrintMonad(PrintLexemeProgress.PL, Lexeme + character, Location.EndAt(position));
                }
                break;

            case PrintLexemeProgress.PL:
                switch (character)
                {
                    case 'n' or 'N': return new MaybePrintMonad(PrintLexemeProgress.PLN, Lexeme + character, Location.EndAt(position));
                }
                break;

            case PrintLexemeProgress.PLN:
                switch (character)
                {
                    case ' ': return new UncompletedPrintMonad(string.Empty, true, Lexeme + character, Location.EndAt(position));
                    case '\n': return new CompletedLexemeMonad(new PrintToken(string.Empty, true, Lexeme, Location), null);
                }
                break;
        }

        return new UnknownLexemeMonad(Lexeme + character, Location.EndAt(position));
    }

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

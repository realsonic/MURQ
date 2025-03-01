using MURQ.URQL.Locations;
using MURQ.URQL.Tokens.Statements;

using static MURQ.URQL.Lexers.Monads.URQL.Print.MaybePlnMonad;

namespace MURQ.URQL.Lexers.Monads.URQL.Print;

public record MaybePlnMonad(PlnLexemeProgress LexemeProgress, string Lexeme, Location Location) : UncompletedLexemeMonad(Lexeme, Location)
{
    public override LexemeMonad Append(char character, Position position)
    {
        if (LexemeProgress == PlnLexemeProgress.PL && character.IsEqualIgnoreCase('n'))
            return new MaybePlnMonad(PlnLexemeProgress.PLN, Lexeme + character, Location.EndAt(position));

        if (LexemeProgress == PlnLexemeProgress.PLN && character is ' ')
            return new UncompletedPrintMonad(string.Empty, true, Lexeme + character, Location.EndAt(position));

        return new UnknownLexemeMonad(Lexeme + character, Location.EndAt(position));
    }

    public override LexemeMonad Finalize()
    {
        if (LexemeProgress == PlnLexemeProgress.PLN)
            return new CompletedLexemeMonad(new PrintToken("\n", Lexeme, Location), null);

        return new UnknownLexemeMonad(Lexeme, Location);
    }

    public enum PlnLexemeProgress
    {
        PL,
        PLN
    }
}

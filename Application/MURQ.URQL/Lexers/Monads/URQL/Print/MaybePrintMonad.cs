using MURQ.URQL.Locations;
using MURQ.URQL.Tokens.Statements;

using static MURQ.URQL.Lexers.Monads.URQL.Print.MaybePlnMonad;

namespace MURQ.URQL.Lexers.Monads.URQL.Print;

public record MaybePrintMonad(string Lexeme, Location Location) : UncompletedLexemeMonad(Lexeme, Location)
{
    public override LexemeMonad Append(char character, Position position)
    {
        if (character is ' ')
            return new UncompletedPrintMonad(string.Empty, false, Lexeme + character, Location.EndAt(position));

        if (character is '\n')
            return new CompletedLexemeMonad(new PrintToken(string.Empty, Lexeme, Location), null);

        if (character.IsEqualIgnoreCase('l'))
            return new MaybePlnMonad(PlnLexemeProgress.PL, Lexeme + character, Location.EndAt(position));

        return new UnknownLexemeMonad(Lexeme + character, Location.EndAt(position));
    }

    public override LexemeMonad Finalize() => new CompletedLexemeMonad(new PrintToken(string.Empty, Lexeme, Location), null);
}

using MURQ.URQL.Parsing.Locations;
using MURQ.URQL.Parsing.Tokens.Statements;

namespace MURQ.URQL.Parsing.Lexers.Monads.URQL.Print;

public record MaybePrintMonad(string Lexeme, Location Location) : UncompletedLexemeMonad(Lexeme, Location)
{
    public override LexemeMonad Append(char character, Position position)
    {
        if (character is ' ')
            return new UncompletedPrintMonad(string.Empty, Lexeme + character, Location.EndAt(position));

        return new UnknownLexemeMonad(Lexeme + character, Location.EndAt(position));
    }

    public override LexemeMonad Finalize() => new CompletedLexemeMonad(new PrintToken(string.Empty, Lexeme, Location), null);
}

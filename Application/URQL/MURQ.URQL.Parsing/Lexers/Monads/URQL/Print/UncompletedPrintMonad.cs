using MURQ.URQL.Parsing.Locations;
using MURQ.URQL.Parsing.Tokens.Statements;

namespace MURQ.URQL.Parsing.Lexers.Monads.URQL.Print;

public record UncompletedPrintMonad(string Text, string Lexeme, Location Location) : UncompletedLexemeMonad(Lexeme, Location)
{
    public override LexemeMonad Append(char character, Position position)
    {
        if (character is '\n')
            return new CompletedLexemeMonad(new PrintToken(Text, Lexeme, Location), null);

        return new UncompletedPrintMonad(Text + character, Lexeme + character, Location.EndAt(position));
    }

    public override LexemeMonad Finalize() => new CompletedLexemeMonad(new PrintToken(Text, Lexeme, Location), null);
}

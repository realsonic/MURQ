using MURQ.URQL.Locations;
using MURQ.URQL.Tokens.Statements;

namespace MURQ.URQL.Lexers.Monads.URQL.Print;

public record UncompletedPrintMonad(string Text, bool IsPlnStatement, string Lexeme, Location Location) : UncompletedLexemeMonad(Lexeme, Location)
{
    public override LexemeMonad Append(char character, Position position) => character switch
    {
        '\n' => new CompletedLexemeMonad(new PrintToken(Text, IsPlnStatement, Lexeme, Location), null),
        _ => new UncompletedPrintMonad(Text + character, IsPlnStatement, Lexeme + character, Location.EndAt(position))
    };

    public override LexemeMonad Finalize() => new CompletedLexemeMonad(new PrintToken(Text, IsPlnStatement, Lexeme, Location), null);
}

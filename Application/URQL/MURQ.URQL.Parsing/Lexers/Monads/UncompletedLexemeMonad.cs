using MURQ.URQL.Parsing.Locations;

namespace MURQ.URQL.Parsing.Lexers.Monads;

public abstract record UncompletedLexemeMonad(string Lexeme, Location Location) : LexemeMonad(Lexeme, Location)
{
    public static LexemeMonad operator +(UncompletedLexemeMonad uncompletedLexemeMonad, (char Character, Position Position) characterAtPosition)
        => uncompletedLexemeMonad.Append(characterAtPosition.Character, characterAtPosition.Position);

    public abstract LexemeMonad Append(char character, Position position);

    public abstract LexemeMonad Finalize();
}
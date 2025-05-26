using MURQ.URQL.Locations;
using MURQ.URQL.Tokens;

namespace MURQ.URQL.Lexers.Monads.URQL;

public record UncompletedVariableMonad(string Lexeme, Location Location) : UncompletedLexemeMonad(Lexeme, Location)
{
    public static UncompletedVariableMonad Start(char startCharacter, Position startPosition) => new(startCharacter.ToString(), Location.StartAt(startPosition));

    public override LexemeMonad Append(char character, Position position) => character switch
    {
        _ when char.IsLetterOrDigit(character) || character is '_' => new UncompletedVariableMonad(Lexeme + character, Location.EndAt(position)),
        _ => new CompletedLexemeMonad(new VariableToken(Lexeme, Lexeme, Location), RootMonad.Remain(character, position))
    };

    public override LexemeMonad Finalize() => new CompletedLexemeMonad(new VariableToken(Lexeme, Lexeme, Location), null);
}

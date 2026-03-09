using MURQ.Domain.URQL.Locations;
using MURQ.Domain.URQL.Tokens;

namespace MURQ.Domain.URQL.Lexing.Monads.URQL;

public record VariableMonad(string Lexeme, Location Location) : UncompletedLexemeMonad(Lexeme, Location)
{
    public static VariableMonad Start(char startCharacter, Position startPosition) => new(startCharacter.ToString(), Location.StartAt(startPosition));

    public override LexemeMonad Append(char character, Position position) => character switch
    {
        '_' => Proceed(character, position),
        _ when char.IsLetterOrDigit(character) => Proceed(character, position),
        _ => new CompletedLexemeMonad(new VariableToken(Lexeme, Lexeme, Location), RootMonad.Remain(character, position))
    };

    public override LexemeMonad Finalize() => new CompletedLexemeMonad(new VariableToken(Lexeme, Lexeme, Location), null);
}

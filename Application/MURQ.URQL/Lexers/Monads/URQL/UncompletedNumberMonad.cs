using MURQ.URQL.Locations;
using MURQ.URQL.Tokens;

namespace MURQ.URQL.Lexers.Monads.URQL;
public record UncompletedNumberMonad(string Lexeme, Location Location) : UncompletedLexemeMonad(Lexeme, Location)
{
    public override LexemeMonad Append(char character, Position position) => character switch
    {
        _ when char.IsNumber(character) => new UncompletedNumberMonad(Lexeme + character, Location.EndAt(position)),
        _ => new CompletedLexemeMonad(new NumberToken(decimal.Parse(Lexeme), Lexeme, Location), RootMonad.Remain(character, position))
    };

    public override LexemeMonad Finalize() => new CompletedLexemeMonad(new NumberToken(decimal.Parse(Lexeme), Lexeme, Location), null);
}
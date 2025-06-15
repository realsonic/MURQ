using MURQ.URQL.Locations;
using MURQ.URQL.Tokens.If;

namespace MURQ.URQL.Lexing.Monads.URQL.Statements.If;
public record ThenMonad(string Lexeme, Location Location) : UncompletedLexemeMonad(Lexeme, Location)
{
    public override LexemeMonad Append(char character, Position position) => character switch
    {
        ' ' or '\t' => new ThenToken(Lexeme, Location).AsMonadWithRemain(character, position),
        _ => new UnknownLexemeMonad(Lexeme + character, Location.EndAt(position), "После then ожидался пробел или табуляция")
    };

    public override LexemeMonad Finalize() => new ThenToken(Lexeme, Location).AsMonad();
}
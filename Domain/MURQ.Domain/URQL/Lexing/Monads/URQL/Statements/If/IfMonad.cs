using MURQ.Domain.URQL.Locations;
using MURQ.Domain.URQL.Tokens.Statements.If;

namespace MURQ.Domain.URQL.Lexing.Monads.URQL.Statements.If;

public record IfMonad(string Lexeme, Location Location) : UncompletedLexemeMonad(Lexeme, Location)
{
    public override LexemeMonad Append(char character, Position position) => character switch
    {
        ' ' or '\t' => new IfToken(Lexeme, Location).AsMonadWithRemain(character, position),
        _ => new UnknownLexemeMonad(Lexeme + character, Location.EndAt(position), "После if ожидался пробел или табуляция")
    };

    public override LexemeMonad Finalize() => new IfToken(Lexeme, Location).AsMonad();
}
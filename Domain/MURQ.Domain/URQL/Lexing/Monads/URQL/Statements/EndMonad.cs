using MURQ.Domain.URQL.Locations;
using MURQ.Domain.URQL.Tokens.Statements;

namespace MURQ.Domain.URQL.Lexing.Monads.URQL.Statements;

public record EndMonad(string Lexeme, Location Location) : UncompletedLexemeMonad(Lexeme, Location)
{
    public override LexemeMonad Append(char character, Position position) => character switch
    {
        ' ' or '\t' or '\n' => new EndToken(Lexeme, Location).AsMonadWithRemain(character, position),
        _ => new UnknownLexemeMonad(Lexeme + character, Location.EndAt(position), "После end ожидался пробел, табуляция или новая строка")
    };

    public override LexemeMonad Finalize() => new EndToken(Lexeme, Location).AsMonad();
}

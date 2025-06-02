using MURQ.URQL.Locations;
using MURQ.URQL.Tokens.Statements;

namespace MURQ.URQL.Lexing.Monads.URQL.Statements;

public record EndMonad(string Lexeme, Location Location) : UncompletedLexemeMonad(Lexeme, Location)
{
    public override LexemeMonad Append(char character, Position position) => character switch
    {
        ' ' or '\t' or '\n' or ';' => new EndToken(Lexeme, Location).AsMonadWithRemain(character, position),
        _ => new UnknownLexemeMonad(Lexeme + character, Location.EndAt(position), "После end ожидался пробел, табуляция, новая строка или комментарий")
    };

    public override LexemeMonad Finalize() => new EndToken(Lexeme, Location).AsMonad();
}

using MURQ.URQL.Locations;
using MURQ.URQL.Tokens.Statements;

namespace MURQ.URQL.Lexing.Monads.URQL.Statements;
public record PerkillMonad(string Lexeme, Location Location) : UncompletedLexemeMonad(Lexeme, Location)
{
    public override LexemeMonad Append(char character, Position position) => character switch
    {
        ' ' or '\t' or '\n' => new PerkillToken(Lexeme, Location).AsMonadWithRemain(character, position),
        _ => new UnknownLexemeMonad(Lexeme + character, Location.EndAt(position), "После perkill ожидался пробел, табуляци или новая строка")
    };

    public override LexemeMonad Finalize() => new PerkillToken(Lexeme, Location).AsMonad();
}

using MURQ.Domain.URQL.Lexing.Monads.URQL;
using MURQ.Domain.URQL.Locations;
using MURQ.Domain.URQL.Tokens;

namespace MURQ.Domain.URQL.Lexing.Monads;

public record CompletedLexemeMonad(Token Token, LexemeMonad? Remain) : LexemeMonad(Token.Lexeme, Token.Location)
{
    public static implicit operator CompletedLexemeMonad(Token token) => new(token);
}

public static class TokenExtensions
{
    public static CompletedLexemeMonad AsMonad(this Token token) => new(token, null);

    public static CompletedLexemeMonad AsMonadWithRemain(this Token token, char character, Position position)
        => new(token, RootMonad.Remain(character, position));
}
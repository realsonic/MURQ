using MURQ.URQL.Tokens;

namespace MURQ.URQL.Lexers.Monads;

public record CompletedLexemeMonad(Token Token, LexemeMonad? Remain) : LexemeMonad(Token.Lexeme, Token.Location)
{
    public static implicit operator CompletedLexemeMonad(Token token) => new(token);
}
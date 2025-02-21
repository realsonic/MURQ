using MURQ.URQL.Parsing.Tokens;

namespace MURQ.URQL.Parsing.Lexers.Monads;

public record CompletedLexemeMonad(Token Token, LexemeMonad? Remain) : LexemeMonad(Token.Lexeme, Token.Location);
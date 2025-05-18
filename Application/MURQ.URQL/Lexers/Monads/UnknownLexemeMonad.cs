using MURQ.URQL.Locations;

namespace MURQ.URQL.Lexers.Monads;

public record UnknownLexemeMonad(string Lexeme, Location Location, string? ErrorMessage = null) : LexemeMonad(Lexeme, Location);
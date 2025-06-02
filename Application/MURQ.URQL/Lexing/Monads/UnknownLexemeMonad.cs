using MURQ.URQL.Locations;

namespace MURQ.URQL.Lexing.Monads;

public record UnknownLexemeMonad(string Lexeme, Location Location, string? ErrorMessage = null) : LexemeMonad(Lexeme, Location);
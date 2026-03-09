using MURQ.Domain.URQL.Locations;

namespace MURQ.Domain.URQL.Lexing.Monads;

public record UnknownLexemeMonad(string Lexeme, Location Location, string? ErrorMessage = null) : LexemeMonad(Lexeme, Location);
using MURQ.URQL.Parsing.Locations;

namespace MURQ.URQL.Parsing.Lexers.Monads;

public record UnknownLexemeMonad(string Lexeme, Location Location) : LexemeMonad(Lexeme, Location);
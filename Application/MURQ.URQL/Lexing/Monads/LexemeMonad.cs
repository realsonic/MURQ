using MURQ.URQL.Locations;

namespace MURQ.URQL.Lexing.Monads;
public abstract record LexemeMonad(string Lexeme, Location Location);

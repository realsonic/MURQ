using MURQ.Domain.URQL.Locations;

namespace MURQ.Domain.URQL.Lexing.Monads;
public abstract record LexemeMonad(string Lexeme, Location Location);

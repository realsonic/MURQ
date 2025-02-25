using MURQ.URQL.Locations;

namespace MURQ.URQL.Lexers.Monads;
public abstract record LexemeMonad(string Lexeme, Location Location);

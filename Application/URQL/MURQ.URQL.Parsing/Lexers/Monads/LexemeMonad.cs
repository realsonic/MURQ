using MURQ.URQL.Parsing.Locations;

namespace MURQ.URQL.Parsing.Lexers.Monads;
public abstract record LexemeMonad(string Lexeme, Location Location);

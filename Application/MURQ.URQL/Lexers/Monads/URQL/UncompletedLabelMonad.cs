using MURQ.URQL.Locations;
using MURQ.URQL.Tokens.Statements;

namespace MURQ.URQL.Lexers.Monads.URQL;

public record UncompletedLabelMonad(string Label, string Lexeme, Location Location) : UncompletedLexemeMonad(Lexeme, Location)
{
    public override LexemeMonad Append(char character, Position position) => character switch
    {
        '\n' when Label.Length > 0 => new CompletedLexemeMonad(new LabelToken(Label, Lexeme, Location), null),
        '\n' when Label.Length is 0 => new UnknownLexemeMonad(Lexeme, Location),
        _ => new UncompletedLabelMonad(Label + character, Lexeme + character, Location.EndAt(position))
    };

    public override LexemeMonad Finalize() => Label.Length switch
    {
        > 0 => new CompletedLexemeMonad(new LabelToken(Label, Lexeme, Location), null),
        _ => new UnknownLexemeMonad(Lexeme, Location)
    };
}
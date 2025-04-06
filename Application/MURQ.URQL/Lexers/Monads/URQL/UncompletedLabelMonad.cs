using MURQ.URQL.Locations;
using MURQ.URQL.Tokens.Statements;

namespace MURQ.URQL.Lexers.Monads.URQL;

public record UncompletedLabelMonad(string Label, string Lexeme, Location Location) : UncompletedLexemeMonad(Lexeme, Location)
{
    public override LexemeMonad Append(char character, Position position) => character switch
    {
        '\n' or ';' => new CompletedLexemeMonad(new LabelToken(Label, Lexeme, Location), RootMonad.Remain(character, position)),
        _ => new UncompletedLabelMonad(Label + character, Lexeme + character, Location.EndAt(position))
    };

    public override LexemeMonad Finalize() => new CompletedLexemeMonad(new LabelToken(Label, Lexeme, Location), null);
}
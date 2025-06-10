using MURQ.URQL.Locations;
using MURQ.URQL.Tokens.Statements;

namespace MURQ.URQL.Lexing.Monads.URQL.Statements;

public record LabelMonad(string Label, string Lexeme, Location Location) : UncompletedLexemeMonad(Lexeme, Location)
{
    public static LabelMonad Start(char startCharacter, Position startPosition)
        => new(string.Empty, startCharacter.ToString(), Location.StartAt(startPosition));

    public override LexemeMonad Append(char character, Position position) => character switch
    {
        '\n' => new CompletedLexemeMonad(new LabelToken(Label, Lexeme, Location), RootMonad.Remain(character, position)),
        _ => new LabelMonad(Label + character, Lexeme + character, Location.EndAt(position))
    };

    public override LexemeMonad Finalize() => new CompletedLexemeMonad(new LabelToken(Label, Lexeme, Location), null);
}
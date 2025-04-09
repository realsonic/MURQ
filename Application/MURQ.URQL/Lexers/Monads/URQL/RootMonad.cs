using MURQ.URQL.Lexers.Monads.URQL.Button;
using MURQ.URQL.Lexers.Monads.URQL.Print;
using MURQ.URQL.Locations;
using MURQ.URQL.Tokens;

using static MURQ.URQL.Lexers.Monads.URQL.Button.MaybeButtonMonad;
using static MURQ.URQL.Lexers.Monads.URQL.MaybeEndMonad;

namespace MURQ.URQL.Lexers.Monads.URQL;

public record RootMonad(Position Position) : UncompletedLexemeMonad(string.Empty, Location.StartAt(Position))
{
    public override LexemeMonad Append(char character, Position position) => character switch
    {
        ' ' or '\t' or '\r' => new RootMonad(position),
        '\n' => new CompletedLexemeMonad(new NewLineToken(character.ToString(), Location.StartAt(position)), null),
        ';' => new UncompletedCommentMonad(character.ToString(), Location.StartAt(position)),
        ':' => new UncompletedLabelMonad(string.Empty, character.ToString(), Location.StartAt(position)),
        'p' or 'P' => new MaybePrintMonad(MaybePrintMonad.PrintLexemeProgress.P, character.ToString(), Location.StartAt(position)),
        'b' or 'B' => new MaybeButtonMonad(MaybeButtonLexemeProgress.B, character.ToString(), Location.StartAt(position)),
        'e' or 'E' => new MaybeEndMonad(EndLexemeProgress.E, character.ToString(), Location.StartAt(position)),
        _ => new UnknownLexemeMonad(character.ToString(), Location.StartAt(position))
    };

    public override LexemeMonad Finalize() => new RootMonad(this);

    public static LexemeMonad Remain(char character, Position position) => new RootMonad(position) + (character, position);
}

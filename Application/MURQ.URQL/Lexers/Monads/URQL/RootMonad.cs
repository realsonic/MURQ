using MURQ.URQL.Lexers.Monads.URQL.Button;
using MURQ.URQL.Lexers.Monads.URQL.Print;
using MURQ.URQL.Locations;

using static MURQ.URQL.Lexers.Monads.URQL.Button.MaybeButtonMonad;

namespace MURQ.URQL.Lexers.Monads.URQL;

public record RootMonad(Position Position) : UncompletedLexemeMonad(string.Empty, Location.StartAt(Position))
{
    public override LexemeMonad Append(char character, Position position) => character switch
    {
        ' ' or '\t' or '\r' or '\n' => new RootMonad(position),
        ':' => new UncompletedLabelMonad(string.Empty, character.ToString(), Location.StartAt(position)),
        'p' or 'P' => new MaybePrintMonad(MaybePrintMonad.PrintLexemeProgress.P, character.ToString(), Location.StartAt(position)),
        'b' or 'B' => new MaybeButtonMonad(ButtonLexemeProgress.B, character.ToString(), Location.StartAt(position)),
        _ => new UnknownLexemeMonad(character.ToString(), Location.StartAt(position))
    };

    public override LexemeMonad Finalize() => new RootMonad(this);

    public static LexemeMonad Remain(char character, Position position) => new RootMonad(position) + (character, position);
}

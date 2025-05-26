using MURQ.URQL.Locations;
using MURQ.URQL.Tokens;

namespace MURQ.URQL.Lexers.Monads.URQL;

public record RootMonad(Position Position) : UncompletedLexemeMonad(string.Empty, Location.StartAt(Position))
{
    public override LexemeMonad Append(char character, Position position) => character switch
    {
        ' ' or '\t' or '\r' => new RootMonad(position),
        '\n' => new NewLineToken(character, position).AsMonad(),
        ';' => new UncompletedCommentMonad(character.ToString(), Location.StartAt(position)),
        ':' => new UncompletedLabelMonad(string.Empty, character.ToString(), Location.StartAt(position)),
        '=' => new EqualityToken(character, position).AsMonad(),
        '_' => UncompletedVariableMonad.Start(character, position),
        _ when char.IsLetter(character) => UncompletedWordMonad.Start(character, position),
        _ when char.IsDigit(character) => new UncompletedNumberMonad(character.ToString(), Location.StartAt(position)),
        _ => new UnknownLexemeMonad(character.ToString(), Location.StartAt(position))
    };

    public override LexemeMonad Finalize() => new RootMonad(this);

    public static LexemeMonad Remain(char character, Position position) => new RootMonad(position) + (character, position);
}

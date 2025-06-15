using MURQ.URQL.Lexing.Monads.URQL.Statements;
using MURQ.URQL.Locations;
using MURQ.URQL.Tokens;

namespace MURQ.URQL.Lexing.Monads.URQL;

public record RootMonad(Position Position) : UncompletedLexemeMonad(string.Empty, Location.StartAt(Position))
{
    public override LexemeMonad Append(char character, Position position) => character switch
    {
        ' ' or '\t' => new RootMonad(position),
        '\n' => new NewLineToken(character, position).AsMonad(),
        ':' => LabelMonad.Start(character, position),
        '=' => new EqualityToken(character, position).AsMonad(),
        '_' => VariableMonad.Start(character, position),
        _ when char.IsLetter(character) => WordMonad.Start(character, position),
        _ when char.IsDigit(character) => NumberMonad.Start(character, position),
        _ => new UnknownLexemeMonad(character.ToString(), Location.StartAt(position))
    };

    public override LexemeMonad Finalize() => new RootMonad(this);

    public static LexemeMonad Remain(char character, Position position) => new RootMonad(position) + (character, position);
}

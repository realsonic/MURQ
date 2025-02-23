using MURQ.URQL.Lexers.Monads.URQL.Print;
using MURQ.URQL.Locations;

namespace MURQ.URQL.Lexers.Monads.URQL;

public record RootMonad(Position Position) : UncompletedLexemeMonad(string.Empty, Location.StartAt(Position))
{
    public override LexemeMonad Append(char character, Position position)
    {
        if (character is ' ' or '\t' or '\r' or '\n')
            return new RootMonad(position);

        if (character.IsEqualIgnoreCase('p'))
            return new MaybePrintMonad(character.ToString(), Location.StartAt(position));

        return new UnknownLexemeMonad(character.ToString(), Location.StartAt(position));
    }

    public override LexemeMonad Finalize() => new RootMonad(this);

    public static LexemeMonad Remain(char character, Position position) => new RootMonad(position) + (character, position);
}

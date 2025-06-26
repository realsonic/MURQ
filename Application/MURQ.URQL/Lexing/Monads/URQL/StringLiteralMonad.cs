using MURQ.URQL.Locations;
using MURQ.URQL.Tokens;

namespace MURQ.URQL.Lexing.Monads.URQL;

public record StringLiteralMonad(string Text, string Lexeme, Location Location) : UncompletedLexemeMonad(Lexeme, Location)
{
    public static StringLiteralMonad StartAfterOpeningQuote(char startCharacter, Position startPosition)
        => new(string.Empty, startCharacter.ToString(), Location.StartAt(startPosition));

    public override LexemeMonad Append(char character, Position position) => character switch
    {
        '\n' => new UnknownLexemeMonad(Lexeme + character, Location.EndAt(position), "В строковом литерале не ожидалась новая строка"),
        '"' => new StringLiteralToken(Text, Lexeme + character, Location.EndAt(position)).AsMonad(),
        _ => new StringLiteralMonad(Text + character, Lexeme + character, Location.EndAt(position))
    };

    public override LexemeMonad Finalize() => new UnknownLexemeMonad(Lexeme, Location, "Строковый литерал не закрыт кавычкой");
}
using MURQ.URQL.Locations;

namespace MURQ.URQL.Lexers.Monads.URQL;

public record WordMonad(string Lexeme, Location Location) : UncompletedLexemeMonad(Lexeme, Location)
{
    public static WordMonad Start(char startCharacter, Position startPosition) => new(startCharacter.ToString(), Location.StartAt(startPosition));

    public override LexemeMonad Append(char character, Position position) => character switch
    {
        '_' or _ when char.IsDigit(character) => new VariableMonad(Lexeme + character, Location.EndAt(position)),
        _ when char.IsLetter(character) => new WordMonad(Lexeme + character, Location.EndAt(position)),
        _ => SpecifyMonad() + (character, position)
    };

    public override LexemeMonad Finalize() => SpecifyMonad().Finalize();

    private UncompletedLexemeMonad SpecifyMonad() => Lexeme.ToLower() switch
    {
        "p" => PrintMonad.StartAfterKeyword(PrintMonad.PrintStatementVariant.P, Lexeme, Location),
        "pln" => PrintMonad.StartAfterKeyword(PrintMonad.PrintStatementVariant.PLN, Lexeme, Location),
        "btn" => ButtonMonad.StartAfterBtn(Lexeme, Location),
        "end" => new EndMonad(Lexeme, Location),
        "cls" => new ClearScreenMonad(Lexeme, Location),
        _ => new VariableMonad(Lexeme, Location)
    };
}
using MURQ.URQL.Locations;

namespace MURQ.URQL.Lexers.Monads.URQL;

public record UncompletedWordMonad(string Lexeme, Location Location) : UncompletedLexemeMonad(Lexeme, Location)
{
    public static UncompletedWordMonad Start(char startCharacter, Position startPosition) => new(startCharacter.ToString(), Location.StartAt(startPosition));

    public override LexemeMonad Append(char character, Position position) => character switch
    {
        '_' or _ when char.IsDigit(character) => new UncompletedVariableMonad(Lexeme + character, Location.EndAt(position)),
        _ when char.IsLetter(character) => new UncompletedWordMonad(Lexeme + character, Location.EndAt(position)),
        _ => SpecifyMonad() + (character, position)
    };

    public override LexemeMonad Finalize() => SpecifyMonad().Finalize();

    private UncompletedLexemeMonad SpecifyMonad() => Lexeme.ToLower() switch
    {
        "p" => UncompletedPrintMonad.StartAfterKeyword(UncompletedPrintMonad.PrintStatementVariant.P, Lexeme, Location),
        "pln" => UncompletedPrintMonad.StartAfterKeyword(UncompletedPrintMonad.PrintStatementVariant.PLN, Lexeme, Location),
        "btn" => UncompletedButtonMonad.StartAfterBtn(Lexeme, Location),
        "end" => new UncompletedEndMonad(Lexeme, Location),
        "cls" => new UncompletedClearScreenMonad(Lexeme, Location),
        _ => new UncompletedVariableMonad(Lexeme, Location)
    };
}
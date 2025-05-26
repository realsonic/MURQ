using MURQ.URQL.Lexers.Monads.URQL.Print;
using MURQ.URQL.Locations;

namespace MURQ.URQL.Lexers.Monads.URQL;

public record UncompletedWordMonad(string Lexeme, Location Location) : UncompletedLexemeMonad(Lexeme, Location)
{
    public static UncompletedWordMonad Start(char startCharacter, Position startPosition) => new(startCharacter.ToString(), Location.StartAt(startPosition));

    public override LexemeMonad Append(char character, Position position) => character switch
    {
        '_' => new MaybeVariableMonad(Lexeme + character, Location.EndAt(position)),
        _ when char.IsLetterOrDigit(character) => new UncompletedWordMonad(Lexeme + character, Location.EndAt(position)),
        _ => SpecifyMonad() + (character, position)
    };

    public override LexemeMonad Finalize() => SpecifyMonad().Finalize();

    private UncompletedLexemeMonad SpecifyMonad() => Lexeme.ToLower() switch
    {
        "p" => UncompletedPrintMonad.StartAfterKeyword(UncompletedPrintMonad.PrintStatementVariant.P, Lexeme, Location),
        "pln" => UncompletedPrintMonad.StartAfterKeyword(UncompletedPrintMonad.PrintStatementVariant.PLN, Lexeme, Location),
        "btn" => UncompletedButtonMonad.StartAfterBtn(Lexeme, Location),
        "end" => new MaybeEndMonad(MaybeEndMonad.EndLexemeProgress.END, Lexeme, Location),
        "cls" => new MaybeClearScreenMonad(MaybeClearScreenMonad.ClearScreenLexemeProgress.CLS, Lexeme, Location),
        _ => new MaybeVariableMonad(Lexeme, Location)
    };
}
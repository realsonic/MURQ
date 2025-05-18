using MURQ.URQL.Lexers.Monads.URQL.Button;
using MURQ.URQL.Lexers.Monads.URQL.Print;
using MURQ.URQL.Locations;

namespace MURQ.URQL.Lexers.Monads.URQL;

public record UncompletedWordMonad(string Lexeme, Location Location) : UncompletedLexemeMonad(Lexeme, Location)
{
    public UncompletedWordMonad(char startCharacter, Position startPosition) : this(startCharacter.ToString(), Location.StartAt(startPosition)) { }

    public override LexemeMonad Append(char character, Position position) => character switch
    {
        '_' => new MaybeVariableMonad(Lexeme + character, Location.EndAt(position)),
        _ when char.IsLetterOrDigit(character) => new UncompletedWordMonad(Lexeme + character, Location.EndAt(position)),
        _ => SpecifyMonad() + (character, position)
    };

    public override LexemeMonad Finalize() => SpecifyMonad().Finalize();

    private UncompletedLexemeMonad SpecifyMonad() => Lexeme.ToLower() switch
    {
        "p" => new MaybePrintMonad(MaybePrintMonad.PrintLexemeProgress.P, Lexeme, Location),
        "pln" => new MaybePrintMonad(MaybePrintMonad.PrintLexemeProgress.PLN, Lexeme, Location),
        "btn" => new MaybeButtonMonad(MaybeButtonMonad.MaybeButtonLexemeProgress.BTN, Lexeme, Location),
        "end" => new MaybeEndMonad(MaybeEndMonad.EndLexemeProgress.END, Lexeme, Location),
        "cls" => new MaybeClearScreenMonad(MaybeClearScreenMonad.ClearScreenLexemeProgress.CLS, Lexeme, Location),
        _ => new MaybeVariableMonad(Lexeme, Location)
    };
}
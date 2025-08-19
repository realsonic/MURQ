using MURQ.URQL.Lexing.Monads.URQL.Statements;
using MURQ.URQL.Lexing.Monads.URQL.Statements.If;
using MURQ.URQL.Locations;

namespace MURQ.URQL.Lexing.Monads.URQL;

public record WordMonad(string Lexeme, Location Location) : UncompletedLexemeMonad(Lexeme, Location)
{
    public static WordMonad Start(char startCharacter, Position startPosition) => new(startCharacter.ToString(), Location.StartAt(startPosition));

    public override LexemeMonad Append(char character, Position position) => character switch
    {
        '_' => new VariableMonad(Lexeme + character, Location.EndAt(position)),
        _ when char.IsDigit(character) => new VariableMonad(Lexeme + character, Location.EndAt(position)),
        _ when char.IsLetter(character) => Proceed(character, position),
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
        "if" => new IfMonad(Lexeme, Location),
        "then" => new ThenMonad(Lexeme, Location),
        "goto" => GotoMonad.StartAfterGoto(Lexeme, Location),
        _ => new VariableMonad(Lexeme, Location)
    };
}
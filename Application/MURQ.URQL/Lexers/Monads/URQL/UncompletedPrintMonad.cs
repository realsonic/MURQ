using MURQ.URQL.Locations;
using MURQ.URQL.Tokens.Statements;

using static MURQ.URQL.Lexers.Monads.URQL.UncompletedPrintMonad;

namespace MURQ.URQL.Lexers.Monads.URQL;

public record UncompletedPrintMonad(PrintLexemeProgress LexemeProgress, PrintStatementVariant StatementVariant, string Text, string Lexeme, Location Location) : UncompletedLexemeMonad(Lexeme, Location)
{
    public static UncompletedPrintMonad StartAfterKeyword(PrintStatementVariant StatementVariant, string Lexeme, Location Location)
        => new(PrintLexemeProgress.JustAfterKeyword, StatementVariant, string.Empty, Lexeme, Location);

    public override LexemeMonad Append(char character, Position position) => LexemeProgress switch
    {
        PrintLexemeProgress.JustAfterKeyword => character switch
        {
            ' ' or '\t' => new UncompletedPrintMonad(PrintLexemeProgress.Text, StatementVariant, string.Empty, Lexeme + character, Location.EndAt(position)),
            '\n' or ';' => new PrintToken(string.Empty, IsNewLineAtEnd(), Lexeme, Location).AsMonadWithRemain(character, position),
            _ => new UnknownLexemeMonad(Lexeme + character, Location.EndAt(position), WhitespaceExpected)
        },

        PrintLexemeProgress.Text => character switch
        {
            '\n' or ';' => new PrintToken(Text, IsNewLineAtEnd(), Lexeme, Location).AsMonadWithRemain(character, position),
            _ => new UncompletedPrintMonad(PrintLexemeProgress.Text, StatementVariant, Text + character, Lexeme + character, Location.EndAt(position))
        },

        _ => throw new LexingException($"Неожиданное состояние монады лексемы печати: {nameof(LexemeProgress)} = {LexemeProgress}")
    };

    public override LexemeMonad Finalize() => new PrintToken(Text, IsNewLineAtEnd(), Lexeme, Location).AsMonad();

    public enum PrintLexemeProgress
    {
        JustAfterKeyword,
        Text
    }

    public enum PrintStatementVariant
    {
        P,
        PLN
    }

    private bool IsNewLineAtEnd() => StatementVariant == PrintStatementVariant.PLN;

    private const string WhitespaceExpected = "После команды p/pln ожидался пробел, табуляция, новая строка или комментарий";
}

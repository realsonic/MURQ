using MURQ.URQL.Locations;
using MURQ.URQL.Tokens.Statements;

namespace MURQ.URQL.Lexers.Monads.URQL.Print;

public record UncompletedPrintMonad(string Text, bool IsPlnStatement, string Lexeme, Location Location) : UncompletedLexemeMonad(Lexeme, Location)
{
    public override LexemeMonad Append(char character, Position position)
    {
        if (character is '\n')
            return new CompletedLexemeMonad(new PrintToken(Text + (IsPlnStatement ? "\n" : string.Empty), Lexeme, Location), null);

        if (character is '\r') // пропускаем возврат каретки (он обычно идёт перед новой строкой \n)
            return new UncompletedPrintMonad(Text, IsPlnStatement, Lexeme, Location);

        return new UncompletedPrintMonad(Text + character, IsPlnStatement, Lexeme + character, Location.EndAt(position));
    }

    public override LexemeMonad Finalize() => new CompletedLexemeMonad(new PrintToken(Text + (IsPlnStatement ? "\n" : string.Empty), Lexeme, Location), null);
}

using MURQ.URQL.Lexing.EnumerableExtensions;
using MURQ.URQL.Lexing.Monads;
using MURQ.URQL.Lexing.Monads.URQL;
using MURQ.URQL.Locations;
using MURQ.URQL.Tokens;

namespace MURQ.URQL.Lexing;

public class UrqlLexer(IEnumerable<char> input)
{
    public Position CurrentPosition { get; private set; } = new(1, 0);

    public IEnumerable<Token> Scan()
    {
        UncompletedLexemeMonad uncompletedMonad = new RootMonad(Position.Initial);

        IEnumerable<(char, Position)> refinedEnumerable = GetRefinedEnumerable();
        foreach ((char character, Position position) in refinedEnumerable)
        {
            LexemeMonad monad = uncompletedMonad + (character, position);

            CurrentPosition = monad.Location.End;

            while (monad is CompletedLexemeMonad completed)
            {
                yield return completed.Token;
                monad = completed.Remain ?? new RootMonad(monad.Location.End);
            }

            switch (monad)
            {
                case UncompletedLexemeMonad uncompleted:
                    uncompletedMonad = uncompleted;
                    break;
                case UnknownLexemeMonad unknown:
                    throw new LexingException($"Неизвестная лексема \"{unknown.Lexeme}\" на {unknown.Location}{GetErrorMessageAppendix(unknown.ErrorMessage)}");
            }
        }

        LexemeMonad finalMonad = uncompletedMonad.Finalize();
        while (finalMonad is CompletedLexemeMonad completed)
        {
            yield return completed.Token;
            finalMonad = completed.Remain ?? new RootMonad(finalMonad.Location.End);
        }

        switch (finalMonad)
        {
            case UncompletedLexemeMonad uncompleted and not RootMonad:
                throw new LexingException($"После финализации монада не завершена: {uncompleted}.");
            case UnknownLexemeMonad unknown:
                throw new LexingException($"Неизвестная лексема \"{unknown.Lexeme}\" на {unknown.Location}{GetErrorMessageAppendix(unknown.ErrorMessage)}");
        }
    }

    private IEnumerable<(char, Position)> GetRefinedEnumerable() => input
        .ToEnumerableWithoutCarriageReturn()
        .ToPositionedEnumerable()
        .ToEnumerableWithoutComments();

    private static string GetErrorMessageAppendix(string? errorMessage) => errorMessage switch
    {
        not null => $": {errorMessage}",
        _ => string.Empty
    };
}

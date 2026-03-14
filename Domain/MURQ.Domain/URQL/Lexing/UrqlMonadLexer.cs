using MURQ.Domain.URQL.Lexing.Exceptions;
using MURQ.Domain.URQL.Lexing.Monads;
using MURQ.Domain.URQL.Lexing.Monads.URQL;
using MURQ.Domain.URQL.Locations;
using MURQ.Domain.URQL.Tokens;

namespace MURQ.Domain.URQL.Lexing;

public class UrqlMonadLexer(IEnumerable<OriginatedCharacter> input)
{
    public Position CurrentPosition { get; private set; } = new(1, 0);

    public IEnumerable<Token> Scan()
    {
        UncompletedLexemeMonad uncompletedMonad = new RootMonad(Position.Initial);

        foreach (OriginatedCharacter originatedCharacter in input)
        {
            Position position = originatedCharacter.Origin switch
            {
                PositionOrigin positionOrigin => positionOrigin.Position,
                LocationOrigin locationOrigin => locationOrigin.Location.Start, // todo заменить ниже на Origin
                _ => throw new NotImplementedException($"{originatedCharacter.Origin.GetType()} пока не поддерживается")
            };
            LexemeMonad monad = uncompletedMonad + (originatedCharacter.Character, position);

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

    private static string GetErrorMessageAppendix(string? errorMessage) => errorMessage switch
    {
        not null => $": {errorMessage}",
        _ => string.Empty
    };
}

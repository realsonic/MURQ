using MURQ.Domain.URQL.Locations;

using System.Collections;

namespace MURQ.Domain.URQL.Lexing.CharacterEnumerableExtensions;
public class CharacterEnumerableWithoutLineContinuation(IEnumerable<PositionedCharacter> enumerable) : IEnumerable<PositionedCharacter>
{
    IEnumerator<PositionedCharacter> IEnumerable<PositionedCharacter>.GetEnumerator() => Enumerate().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => Enumerate().GetEnumerator();

    private IEnumerable<PositionedCharacter> Enumerate()
    {
        ContinuationState breakState = ContinuationState.NotInContinuation;

        Queue<PositionedCharacter> charQueue = new();

        foreach ((char character, Position position) in enumerable)
        {
            switch (breakState)
            {
                case ContinuationState.NotInContinuation:
                    switch (character)
                    {
                        case '\n':
                            breakState = ContinuationState.NewLineMet;
                            charQueue.Enqueue(new(character, position));
                            break;

                        default:
                            yield return new(character, position);
                            break;
                    }
                    break;

                case ContinuationState.NewLineMet:
                    switch (character)
                    {
                        case ' ' or '\t':
                            charQueue.Enqueue(new(character, position));
                            break;

                        case '_':
                            charQueue.Enqueue(new(character, position));
                            breakState = ContinuationState.UnderscoreMet;
                            break;

                        default:
                            charQueue.Enqueue(new(character, position));
                            while (charQueue.TryDequeue(out PositionedCharacter @char))
                            {
                                yield return @char;
                            }
                            breakState = ContinuationState.NotInContinuation;
                            break;
                    }
                    break;

                case ContinuationState.UnderscoreMet:
                    switch (character)
                    {
                        case ' ' or '\t':
                            charQueue.Clear();
                            yield return new(character, position);
                            breakState = ContinuationState.NotInContinuation;
                            break;

                        default:
                            charQueue.Enqueue(new(character, position));
                            while (charQueue.TryDequeue(out PositionedCharacter @char))
                            {
                                yield return @char;
                            }
                            breakState = ContinuationState.NotInContinuation;
                            break;
                    }
                    break;

                default:
                    throw new NotImplementedException($"Неизвестное состояние: {breakState}");
            }
        }

        while (charQueue.TryDequeue(out PositionedCharacter @char))
        {
            yield return @char;
        }
    }

    private enum ContinuationState
    {
        NotInContinuation,
        NewLineMet,
        UnderscoreMet
    }
}

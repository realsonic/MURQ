using MURQ.URQL.Locations;

using System.Collections;

namespace MURQ.URQL.Lexing.EnumerableExtensions;
public class EnumerableWithoutLineContinuation(IEnumerable<(char Character, Position Position)> enumerable) : IEnumerable<(char Character, Position Position)>
{
    IEnumerator<(char Character, Position Position)> IEnumerable<(char Character, Position Position)>.GetEnumerator() => Enumerate().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => Enumerate().GetEnumerator();

    private IEnumerable<(char Character, Position Position)> Enumerate()
    {
        ContinuationState breakState = ContinuationState.NotInContinuation;

        Queue<(char Character, Position Position)> charQueue = new();

        foreach ((char character, Position position) in enumerable)
        {
            switch (breakState)
            {
                case ContinuationState.NotInContinuation:
                    switch (character)
                    {
                        case '\n':
                            breakState = ContinuationState.NewLineMet;
                            charQueue.Enqueue((character, position));
                            break;

                        default:
                            yield return (character, position);
                            break;
                    }
                    break;

                case ContinuationState.NewLineMet:
                    switch (character)
                    {
                        case ' ' or '\t':
                            charQueue.Enqueue((character, position));
                            break;

                        case '_':
                            charQueue.Enqueue((character, position));
                            breakState = ContinuationState.UnderscoreMet;
                            break;

                        default:
                            charQueue.Enqueue((character, position));
                            while (charQueue.TryDequeue(out (char, Position) @char))
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
                            yield return (character, position);
                            breakState = ContinuationState.NotInContinuation;
                            break;

                        default:
                            charQueue.Enqueue((character, position));
                            while (charQueue.TryDequeue(out (char, Position) @char))
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

        while (charQueue.TryDequeue(out (char, Position) @char))
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

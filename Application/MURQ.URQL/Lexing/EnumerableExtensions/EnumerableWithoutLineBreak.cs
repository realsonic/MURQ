using MURQ.URQL.Locations;

using System.Collections;

namespace MURQ.URQL.Lexing.EnumerableExtensions;
public class EnumerableWithoutLineBreak(IEnumerable<(char, Position)> enumerable) : IEnumerable<(char, Position)>
{
    IEnumerator<(char, Position)> IEnumerable<(char, Position)>.GetEnumerator() => Enumerate().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => Enumerate().GetEnumerator();

    private IEnumerable<(char, Position)> Enumerate()
    {
        BreakState breakState = BreakState.NotInBreak;
        Queue<(char, Position)> charQueue = new();

        foreach ((char character, Position position) in enumerable)
        {
            switch (breakState)
            {
                case BreakState.NotInBreak:
                    switch (character)
                    {
                        case '\n':
                            breakState = BreakState.LfMet;
                            charQueue.Enqueue((character, position));
                            break;

                        default:
                            yield return (character, position);
                            break;
                    }
                    break;

                case BreakState.LfMet:
                    switch (character)
                    {
                        case ' ' or '\t':
                            charQueue.Enqueue((character, position));
                            break;

                        case '_':
                            breakState = BreakState.UnderscoreMet;
                            charQueue.Enqueue((character, position));
                            break;
                        
                        default:
                            breakState = BreakState.NotInBreak;
                            foreach (var @char in charQueue)
                            {
                                yield return @char;
                            }
                            break;
                    }
                    break;

                case BreakState.UnderscoreMet:
                    switch (character)
                    {
                        case ' ' or '\t':
                            charQueue.Clear();
                            breakState = BreakState.NotInBreak;
                            yield return (character, position);
                            break;
                        
                        default:
                            breakState = BreakState.NotInBreak;
                            foreach (var @char in charQueue)
                            {
                                yield return @char;
                            }
                            break;
                    }
                    break;
                
                default:
                    throw new NotImplementedException($"Неизвестное состояние: {breakState}");
            }
        }
    }

    private enum BreakState
    {
        NotInBreak,
        LfMet,
        UnderscoreMet
    }
}

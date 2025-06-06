using MURQ.URQL.Locations;

using System.Collections;

namespace MURQ.URQL.Lexing.EnumerableExtensions;

public class EnumerableWithoutComments(IEnumerable<(char, Position)> enumerable) : IEnumerable<(char, Position)>
{
    IEnumerator<(char, Position)> IEnumerable<(char, Position)>.GetEnumerator() => Enumerate().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => Enumerate().GetEnumerator();

    private IEnumerable<(char, Position)> Enumerate()
    {
        CommentState commentState = CommentState.NotInComment;

        (char, Position)? postponedCharacter = null;

        foreach ((char character, Position position) in enumerable)
        {
            switch (commentState)
            {
                case CommentState.NotInComment:
                    if (character is ';')
                    {
                        commentState = CommentState.InSinglelineComment;
                        continue;
                    }
                    else if (character is '/')
                    {
                        commentState = CommentState.MaybeMultilineCommentStarting;
                        postponedCharacter = (character, position);
                        continue;
                    }
                    else
                    {
                        yield return (character, position);
                    }
                    break;

                case CommentState.InSinglelineComment:
                    if (character is '\n')
                    {
                        commentState = CommentState.NotInComment;
                        yield return (character, position);
                    }
                    else
                    {
                        continue;
                    }
                    break;

                case CommentState.MaybeMultilineCommentStarting:
                    if (character is '*')
                    {
                        commentState = CommentState.InMultilineComment;
                        postponedCharacter = null;
                        continue;
                    }
                    else
                    {
                        commentState = CommentState.NotInComment;
                        yield return postponedCharacter ?? throw new LexingException($"Неожиданно не задан отложенный символ в состоянии {commentState}");
                        postponedCharacter = null;
                    }
                    break;

                case CommentState.InMultilineComment:
                    if (character is '*')
                    {
                        commentState = CommentState.MaybeMultilineCommentEnding;
                        continue;
                    }
                    else
                    {
                        continue;
                    }

                case CommentState.MaybeMultilineCommentEnding:
                    if (character is '/')
                    {
                        commentState = CommentState.NotInComment;
                        continue;
                    }
                    else
                    {
                        continue;
                    }

                default:
                    throw new NotImplementedException($"Неизвестное состояние: {commentState}");
            }
        }

        // когда поток закончился, а отложенный символ есть, нужно вернуть его состояние:
        if (postponedCharacter is not null)
        {
            yield return postponedCharacter.Value;
        }
    }

    private enum CommentState
    {
        NotInComment,
        InSinglelineComment,
        MaybeMultilineCommentStarting,
        InMultilineComment,
        MaybeMultilineCommentEnding
    }
}
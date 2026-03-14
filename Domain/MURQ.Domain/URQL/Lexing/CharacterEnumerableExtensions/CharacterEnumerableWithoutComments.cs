using MURQ.Domain.URQL.Lexing.Exceptions;
using MURQ.Domain.URQL.Locations;

using System.Collections;

namespace MURQ.Domain.URQL.Lexing.CharacterEnumerableExtensions;

public class CharacterEnumerableWithoutComments(IEnumerable<PositionedCharacter> enumerable) : IEnumerable<PositionedCharacter>
{
    IEnumerator<PositionedCharacter> IEnumerable<PositionedCharacter>.GetEnumerator() => Enumerate().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => Enumerate().GetEnumerator();

    private IEnumerable<PositionedCharacter> Enumerate()
    {
        CommentState commentState = CommentState.NotInComment;

        PositionedCharacter? postponedCharacter = null;

        foreach ((char character, Position position) in enumerable)
        {
            switch (commentState)
            {
                case CommentState.NotInComment:
                    if (character is ';')
                    {
                        commentState = CommentState.InSinglelineComment;
                    }
                    else if (character is '/')
                    {
                        commentState = CommentState.MaybeMultilineCommentStarting;
                        postponedCharacter = new(character, position);
                    }
                    else
                    {
                        yield return new(character, position);
                    }
                    break;

                case CommentState.InSinglelineComment:
                    if (character is '\n')
                    {
                        commentState = CommentState.NotInComment;
                        yield return new(character, position);
                    }
                    break;

                case CommentState.MaybeMultilineCommentStarting:
                    if (character is '*')
                    {
                        commentState = CommentState.InMultilineComment;
                        postponedCharacter = null;
                    }
                    else
                    {
                        commentState = CommentState.NotInComment;
                        yield return postponedCharacter ?? throw new LexingException($"Неожиданно не задан отложенный символ в состоянии {commentState}");
                        postponedCharacter = null;
                        yield return new(character, position);
                    }
                    break;

                case CommentState.InMultilineComment:
                    if (character is '*')
                    {
                        commentState = CommentState.MaybeMultilineCommentEnding;
                    }
                    break;

                case CommentState.MaybeMultilineCommentEnding:
                    if (character is '/')
                    {
                        commentState = CommentState.NotInComment;
                    }
                    break;

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
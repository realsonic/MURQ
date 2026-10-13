using MURQ.Domain.URQL.Tokens;

namespace MURQ.Domain.URQL.Lexing.Exceptions;

[Serializable]
internal class UnknownLexemeException : LexingException
{
    public UnknownLexemeException(string? message) : base(message)
    {
    }

    public UnknownLexemeException(Lexeme lexeme) : base($"Неизвестная лексема \"{lexeme}\"", lexeme.Location)
    {
    }

    public UnknownLexemeException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
using MURQ.Domain.URQL.Locations;
using MURQ.Domain.URQL.Tokens;

namespace MURQ.Domain.URQL.Lexing.Exceptions;

[Serializable]
internal class LexingException : UrqlException
{
    public LexingException(string? message) : base(message)
    {
    }

    public LexingException(string? message, Location location) : base($"{message} на {location}")
    {
    }

    public LexingException(string? message, Lexeme lexeme) : this($"{message}: {lexeme.Literal}", lexeme.Location)
    {
    }

    public LexingException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}

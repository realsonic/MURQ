using MURQ.Domain.URQL.Locations;

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

    public LexingException(string? message, (string Lexeme, Location Location) lexemeData) : this($"{message}: {lexemeData.Lexeme}", lexemeData.Location)
    {
    }

    public LexingException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}

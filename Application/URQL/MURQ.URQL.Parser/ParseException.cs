using MURQ.Domain.Exceptions;

namespace MURQ.URQL.Parser;

[Serializable]
internal class ParseException : MurqException
{
    public ParseException(string? message) : base(message)
    {
    }

    public ParseException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
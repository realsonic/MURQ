namespace MURQ.Domain.URQL.Interpretation.Exceptions;

[Serializable]
public class ParseException : UrqlException
{
    public ParseException(string? message) : base(message)
    {
    }

    public ParseException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
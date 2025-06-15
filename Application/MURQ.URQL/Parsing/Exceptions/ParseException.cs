namespace MURQ.URQL.Parsing.Exceptions;

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
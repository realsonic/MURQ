using MURQ.Domain.Exceptions;

namespace MURQ.URQL.Parsing;

[Serializable]
public class UrqlException : MurqException
{
    public UrqlException(string? message) : base(message)
    {
    }

    public UrqlException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
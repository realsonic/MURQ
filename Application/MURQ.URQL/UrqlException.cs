using MURQ.Common.Exceptions;

namespace MURQ.URQL;

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
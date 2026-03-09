using MURQ.Domain.URQL;

namespace MURQ.Domain.URQL.Interpretation.Exceptions;

[Serializable]
public class InterpretationException : UrqlException
{
    public InterpretationException(string? message) : base(message)
    {
    }

    public InterpretationException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
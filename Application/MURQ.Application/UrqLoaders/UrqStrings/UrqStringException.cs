using MURQ.URQL;

namespace MURQ.Application.UrqLoaders.UrqStrings;

[Serializable]
public class UrqStringException : UrqlException
{
    public UrqStringException(string? message) : base(message)
    {
    }

    public UrqStringException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}

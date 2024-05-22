namespace MURQ.Domain.Exceptions;

/// <summary>
/// Базовая ошибка домена MURQ.
/// </summary>
public class MurqException : Exception
{
    public MurqException(string? message) : base(message)
    {
    }

    public MurqException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
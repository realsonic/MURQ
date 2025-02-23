namespace MURQ.URQL.Lexers;

[Serializable]
internal class LexingException : UrqlException
{
    public LexingException(string? message) : base(message)
    {
    }

    public LexingException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
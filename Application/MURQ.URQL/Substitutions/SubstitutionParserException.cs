namespace MURQ.URQL.Substitutions;

[Serializable]
public class SubstitutionParserException : UrqlException
{
    public SubstitutionParserException(string? message) : base(message)
    {
    }

    public SubstitutionParserException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}

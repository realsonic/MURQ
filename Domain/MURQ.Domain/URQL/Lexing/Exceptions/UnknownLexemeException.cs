using MURQ.Domain.URQL.Locations;

namespace MURQ.Domain.URQL.Lexing.Exceptions;

[Serializable]
internal class UnknownLexemeException : LexingException
{
    public UnknownLexemeException(string? message) : base(message)
    {
    }

    public UnknownLexemeException(string lexeme, Location location) : base($"Неизвестная лексема \"{lexeme}\"", location)
    {
    }

    public UnknownLexemeException((string Lexeme, Location Location) lexemeData) : this(lexemeData.Lexeme, lexemeData.Location)
    {
    }

    public UnknownLexemeException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
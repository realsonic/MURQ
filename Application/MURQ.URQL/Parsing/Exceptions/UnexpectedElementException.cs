using MURQ.URQL.Tokens;

namespace MURQ.URQL.Parsing.Exceptions;

[Serializable]
public class UnexpectedElementException(string expectedElement, Token? metToken, string? context = null, string? message = null)
    : ParseException(message ?? FormatDefaultMessage(expectedElement, metToken, context))
{
    public Token? MetToken { get; } = metToken;

    private static string FormatDefaultMessage(string expectedElement, Token? metToken, string? context)
        => $"{expectedElement}{FormatContext(context)}, а встретился токен {FormatMetToken(metToken)}";

    private static string FormatContext(string? context) => context switch
    {
        not null => $" {context}",
        _ => string.Empty
    };

    private static string FormatMetToken(Token? metToken) => metToken switch
    {
        not null => $"<{metToken.GetDescription()}> на {metToken.Location}",
        _ => "конец"
    };
}
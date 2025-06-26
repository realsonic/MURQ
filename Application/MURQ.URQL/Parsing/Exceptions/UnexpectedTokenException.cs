using MURQ.URQL.Tokens;

namespace MURQ.URQL.Parsing.Exceptions;

[Serializable]
public class UnexpectedTokenException<TExpectedToken>(Token? metToken, string? context = null, string? message = null)
    : ParseException(message ?? FormatDefaultMessage(metToken, context))
    where TExpectedToken : Token
{
    public Token? MetToken { get; } = metToken;

    private static string FormatDefaultMessage(Token? metToken, string? context)
        => $"Ожидался токен {FormatExpectedToken()}{FormatContext(context)}, а встретился {FormatMetToken(metToken)}";

    private static string FormatExpectedToken() => $"<{Token.GetTokenTypeDescription<TExpectedToken>()}>";

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
using MURQ.URQL.Locations;

using System.ComponentModel;
using System.Reflection;

namespace MURQ.URQL.Tokens;

public abstract record Token(string Lexeme, Location Location)
{
    public static string GetTokenTypeDescription<TToken>() where TToken : Token
    {
        DescriptionAttribute? descriptionAttribute = typeof(TToken).GetCustomAttribute<DescriptionAttribute>();
        return descriptionAttribute?.Description ?? typeof(TToken).Name;
    }

    public virtual string GetDescription() => ToString();
}
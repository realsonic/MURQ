using MURQ.Domain.URQL.Locations;

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace MURQ.Domain.URQL.Tokens;

public abstract record Token(string Lexeme, Location Location)
{
    public static string GetTokenTypeDescription<TToken>() where TToken : Token
    {
        DescriptionAttribute? descriptionAttribute = typeof(TToken).GetCustomAttribute<DescriptionAttribute>();
        return descriptionAttribute?.Description ?? typeof(TToken).Name;
    }

    public virtual string Description => TryGetDescriptionFromAttribute(out string? description) ? description : ToString();

    private bool TryGetDescriptionFromAttribute([NotNullWhen(true)] out string? description)
    {
        DescriptionAttribute? descriptionAttribute = GetType().GetCustomAttribute<DescriptionAttribute>();

        if (descriptionAttribute != null)
        {
            description = descriptionAttribute.Description;
            return true;
        }

        description = null;
        return false;
    }
}
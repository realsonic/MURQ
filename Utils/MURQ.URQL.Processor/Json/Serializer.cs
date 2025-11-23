using MURQ.URQL.Locations;
using MURQ.URQL.Substitutions;

using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

using static MURQ.URQL.Substitutions.SubstitutionTree;

namespace MURQ.URQL.Processor.Json;
internal class Serializer
{
    public static string Serialize(SubstitutionTree tree)
        => JsonSerializer.Serialize(tree, jsonSerializerOptions);

    static readonly JsonSerializerOptions jsonSerializerOptions = new()
    {
        TypeInfoResolver = GetJsonTypeInfoResolver(),
        Converters = { new JsonStringEnumConverter(), new ToStringConverter<Location>() },
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    static DefaultJsonTypeInfoResolver GetJsonTypeInfoResolver()
    {
        var resolver = new DefaultJsonTypeInfoResolver();

        resolver.Modifiers.Add(typeInfo =>
        {
            if (typeInfo.Type == typeof(Node))
            {
                typeInfo.PolymorphismOptions = new JsonPolymorphismOptions
                {
                    TypeDiscriminatorPropertyName = "Type",
                    UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization,
                    DerivedTypes =
                    {
                        new JsonDerivedType(typeof(StringNode), "String"),
                        new JsonDerivedType(typeof(SubstitutionNode), "Substitution")
                    }
                };
            }
        });

        return resolver;
    }
}
using MURQ.Domain.Quests.QuestLines;
using MURQ.Domain.Quests.QuestLines.SubstitutionTrees;
using MURQ.Domain.URQL.Locations;

using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace MURQ.URQL.Processor.Json;

internal class Serializer
{
    public static string Serialize(CodeLine codeLine)
        => JsonSerializer.Serialize(codeLine, jsonSerializerOptions);

    static readonly JsonSerializerOptions jsonSerializerOptions = new()
    {
        TypeInfoResolver = GetJsonTypeInfoResolver(),
        Converters = { new JsonStringEnumConverter(), new LocationConverter() },
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    static DefaultJsonTypeInfoResolver GetJsonTypeInfoResolver()
    {
        var resolver = new DefaultJsonTypeInfoResolver();

        resolver.Modifiers.Add(typeInfo =>
        {
            if (typeInfo.Type == typeof(TreeNode))
            {
                typeInfo.PolymorphismOptions = new JsonPolymorphismOptions
                {
                    TypeDiscriminatorPropertyName = "Type",
                    UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization,
                    DerivedTypes =
                    {
                        new JsonDerivedType(typeof(CodeNode), "String"),
                        new JsonDerivedType(typeof(SubstitutionNode), "Substitution")
                    }
                };
            }
        });

        return resolver;
    }
}

public class LocationConverter : JsonConverter<Location>
{
    public override Location Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, Location value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}

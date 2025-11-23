using System.Text.Json;
using System.Text.Json.Serialization;

namespace MURQ.URQL.Processor.Json;

class ToStringConverter<T> : JsonConverter<T> where T : class
{
    public override bool CanConvert(Type typeToConvert) =>
        typeof(T) == typeToConvert;

    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        //string str = reader.GetString();
        // Здесь нужно восстановить объект из строки (если требуется)
        throw new NotImplementedException("Десериализация не реализована");
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        if (value == null)
            writer.WriteNullValue();
        else
            writer.WriteStringValue(value.ToString());
    }
}

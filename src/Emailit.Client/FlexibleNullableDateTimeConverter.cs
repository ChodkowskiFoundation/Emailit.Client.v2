using System.Text.Json;
using System.Text.Json.Serialization;

namespace Emailit.Client;

internal sealed class FlexibleNullableDateTimeConverter : JsonConverter<DateTime?>
{
    private static readonly FlexibleDateTimeConverter Inner = new();

    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        return Inner.Read(ref reader, typeof(DateTime), options);
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            Inner.Write(writer, value.Value, options);
            return;
        }

        writer.WriteNullValue();
    }
}

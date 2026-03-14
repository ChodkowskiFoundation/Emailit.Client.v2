using System.Text.Json;
using System.Text.Json.Serialization;

namespace Emailit.Client;

internal sealed class FlexibleStringListConverter : JsonConverter<List<string>?>
{
    public override List<string>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.Null => null,
            JsonTokenType.String => [reader.GetString() ?? string.Empty],
            JsonTokenType.StartArray => ReadArray(ref reader),
            _ => throw new JsonException($"Expected string or string array token, got {reader.TokenType}.")
        };
    }

    public override void Write(Utf8JsonWriter writer, List<string>? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartArray();
        foreach (var item in value)
        {
            writer.WriteStringValue(item);
        }

        writer.WriteEndArray();
    }

    private static List<string> ReadArray(ref Utf8JsonReader reader)
    {
        var values = new List<string>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
            {
                return values;
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                values.Add(reader.GetString() ?? string.Empty);
                continue;
            }

            throw new JsonException($"Expected string item in array, got {reader.TokenType}.");
        }

        throw new JsonException("Incomplete JSON array while reading string list.");
    }
}

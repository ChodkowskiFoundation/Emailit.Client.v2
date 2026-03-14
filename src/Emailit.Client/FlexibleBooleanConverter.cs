using System.Text.Json;
using System.Text.Json.Serialization;

namespace Emailit.Client;

internal sealed class FlexibleBooleanConverter : JsonConverter<bool>
{
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.TokenType switch
        {
            JsonTokenType.True => true,
            JsonTokenType.False => false,
            JsonTokenType.Number when reader.TryGetInt64(out var numeric) => numeric switch
            {
                0 => false,
                1 => true,
                _ => throw new JsonException($"Expected boolean-compatible number, got {numeric}.")
            },
            JsonTokenType.String => ParseString(reader.GetString()),
            _ => throw new JsonException($"Expected boolean-compatible token, got {reader.TokenType}.")
        };

    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options) =>
        writer.WriteBooleanValue(value);

    private static bool ParseString(string? value)
    {
        if (bool.TryParse(value, out var boolValue))
        {
            return boolValue;
        }

        if (long.TryParse(value, out var numeric))
        {
            return numeric switch
            {
                0 => false,
                1 => true,
                _ => throw new JsonException($"Expected boolean-compatible number, got {numeric}.")
            };
        }

        throw new JsonException($"Expected boolean-compatible string, got '{value}'.");
    }
}

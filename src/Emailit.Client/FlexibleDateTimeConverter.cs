using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Emailit.Client;

internal sealed class FlexibleDateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.TokenType switch
        {
            JsonTokenType.String => ParseString(reader.GetString()),
            JsonTokenType.Number when reader.TryGetInt64(out var numeric) => ParseUnix(numeric),
            _ => throw new JsonException($"Expected DateTime-compatible token, got {reader.TokenType}.")
        };

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture));

    private static DateTime ParseString(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return default;
        }

        if (DateTime.TryParse(
            value,
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
            out var parsed))
        {
            return parsed;
        }

        if (long.TryParse(value, out var unix))
        {
            return ParseUnix(unix);
        }

        throw new JsonException($"Expected DateTime-compatible string, got '{value}'.");
    }

    private static DateTime ParseUnix(long value)
    {
        var dateTimeOffset = Math.Abs(value) >= 1_000_000_000_000
            ? DateTimeOffset.FromUnixTimeMilliseconds(value)
            : DateTimeOffset.FromUnixTimeSeconds(value);

        return dateTimeOffset.UtcDateTime;
    }
}

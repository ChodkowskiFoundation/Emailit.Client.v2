using System.Text.Json;
using System.Text.Json.Serialization;

namespace Emailit.Client.Models;

/// <summary>
/// Error response from the Emailit API.
/// </summary>
public sealed record ErrorResponse
{
    [JsonPropertyName("error")]
    public string? Error { get; init; }

    [JsonPropertyName("message")]
    public string? Message { get; init; }

    [JsonPropertyName("code")]
    public string? Code { get; init; }

    [JsonPropertyName("details")]
    public string? Details { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("title")]
    public string? Title { get; init; }

    [JsonPropertyName("detail")]
    public string? Detail { get; init; }

    [JsonPropertyName("instance")]
    public string? Instance { get; init; }

    [JsonPropertyName("errors")]
    public JsonElement? Errors { get; init; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; init; }

    /// <summary>
    /// Gets the best available error message.
    /// </summary>
    public string GetErrorMessage()
    {
        return Message ?? Detail ?? Title ?? Error ?? Code ?? Details ?? "Unknown error";
    }

    /// <summary>
    /// Extracts validation errors from heterogeneous API payloads.
    /// </summary>
    public IReadOnlyDictionary<string, string[]>? GetValidationErrors()
    {
        if (Errors is not { ValueKind: JsonValueKind.Object } errors)
            return null;

        var result = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);

        foreach (var property in errors.EnumerateObject())
        {
            var messages = property.Value.ValueKind switch
            {
                JsonValueKind.Array => property.Value.EnumerateArray()
                    .Where(item => item.ValueKind == JsonValueKind.String)
                    .Select(item => item.GetString())
                    .Where(item => !string.IsNullOrWhiteSpace(item))
                    .Cast<string>()
                    .ToArray(),
                JsonValueKind.String => [property.Value.GetString()!],
                _ => []
            };

            if (messages.Length > 0)
                result[property.Name] = messages;
        }

        return result.Count > 0 ? result : null;
    }
}

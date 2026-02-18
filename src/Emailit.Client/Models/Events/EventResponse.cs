using System.Text.Json;
using System.Text.Json.Serialization;

namespace Emailit.Client.Models.Events;

/// <summary>
/// Response model for event operations.
/// </summary>
public sealed record EventResponse
{
    /// <summary>
    /// Object type identifier.
    /// </summary>
    [JsonPropertyName("object")]
    public string Object { get; init; } = "event";

    /// <summary>
    /// Unique event identifier (prefixed with evt_).
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = null!;

    /// <summary>
    /// Event type (e.g., "email.accepted", "email.delivered", "contact.created").
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; init; } = null!;

    /// <summary>
    /// Event payload data. Only present when include_data=true.
    /// Uses JsonElement to support varying payload structures.
    /// </summary>
    [JsonPropertyName("data")]
    public JsonElement? Data { get; init; }

    /// <summary>
    /// Event creation timestamp.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; init; }
}

using System.Text.Json.Serialization;

namespace Emailit.Client.Models.Webhooks;

/// <summary>
/// Response model for webhook operations.
/// </summary>
public sealed record WebhookResponse
{
    /// <summary>
    /// Object type identifier.
    /// </summary>
    [JsonPropertyName("object")]
    public string Object { get; init; } = "webhook";

    /// <summary>
    /// Unique webhook identifier (prefixed with wh_).
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = null!;

    /// <summary>
    /// Webhook name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = null!;

    /// <summary>
    /// HTTPS endpoint URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; init; } = null!;

    /// <summary>
    /// Whether the webhook is subscribed to all events.
    /// </summary>
    [JsonPropertyName("all_events")]
    public bool AllEvents { get; init; }

    /// <summary>
    /// Whether the webhook is active.
    /// </summary>
    [JsonPropertyName("enabled")]
    public bool Enabled { get; init; }

    /// <summary>
    /// Subscribed event types.
    /// </summary>
    [JsonPropertyName("events")]
    public List<string>? Events { get; init; }

    /// <summary>
    /// Timestamp of last webhook delivery.
    /// </summary>
    [JsonPropertyName("last_used_at")]
    public DateTime? LastUsedAt { get; init; }

    /// <summary>
    /// Creation timestamp.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// Last update timestamp.
    /// </summary>
    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; init; }
}

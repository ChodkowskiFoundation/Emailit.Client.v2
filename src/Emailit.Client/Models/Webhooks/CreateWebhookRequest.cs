using System.Text.Json.Serialization;

namespace Emailit.Client.Models.Webhooks;

/// <summary>
/// Request model for creating a webhook.
/// </summary>
public sealed class CreateWebhookRequest
{
    /// <summary>
    /// Webhook name. Must be unique within workspace.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// HTTPS endpoint URL to receive webhook events.
    /// </summary>
    [JsonPropertyName("url")]
    public required string Url { get; init; }

    /// <summary>
    /// Subscribe to all event types.
    /// </summary>
    [JsonPropertyName("all_events")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? AllEvents { get; init; }

    /// <summary>
    /// Whether the webhook is active.
    /// </summary>
    [JsonPropertyName("enabled")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Enabled { get; init; }

    /// <summary>
    /// Array of event types to subscribe to (e.g., "email.delivered", "email.bounced").
    /// </summary>
    [JsonPropertyName("events")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyList<string>? Events { get; init; }
}

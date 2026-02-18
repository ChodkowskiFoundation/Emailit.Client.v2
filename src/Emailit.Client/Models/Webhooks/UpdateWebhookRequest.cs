using System.Text.Json.Serialization;

namespace Emailit.Client.Models.Webhooks;

/// <summary>
/// Request model for updating a webhook.
/// </summary>
public sealed class UpdateWebhookRequest
{
    /// <summary>
    /// New webhook name.
    /// </summary>
    [JsonPropertyName("name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Name { get; init; }

    /// <summary>
    /// New HTTPS endpoint URL.
    /// </summary>
    [JsonPropertyName("url")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Url { get; init; }

    /// <summary>
    /// Subscribe to all event types. Setting true clears specific events.
    /// </summary>
    [JsonPropertyName("all_events")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? AllEvents { get; init; }

    /// <summary>
    /// Enable or disable the webhook.
    /// </summary>
    [JsonPropertyName("enabled")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Enabled { get; init; }

    /// <summary>
    /// Replace subscribed event types. Ignored when all_events is true.
    /// </summary>
    [JsonPropertyName("events")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyList<string>? Events { get; init; }
}

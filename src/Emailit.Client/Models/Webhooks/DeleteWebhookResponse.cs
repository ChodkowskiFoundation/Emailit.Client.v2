using System.Text.Json.Serialization;

namespace Emailit.Client.Models.Webhooks;

/// <summary>
/// Response for webhook deletion.
/// </summary>
public sealed record DeleteWebhookResponse
{
    /// <summary>
    /// Object type identifier.
    /// </summary>
    [JsonPropertyName("object")]
    public string Object { get; init; } = "webhook";

    /// <summary>
    /// Webhook ID that was deleted.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = null!;

    /// <summary>
    /// Webhook name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = null!;

    /// <summary>
    /// Whether deletion was successful.
    /// </summary>
    [JsonPropertyName("deleted")]
    public bool Deleted { get; init; }
}

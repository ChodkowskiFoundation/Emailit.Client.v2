using System.Text.Json.Serialization;

namespace Emailit.Client.Models.Contacts;

/// <summary>
/// Subscriber information nested within a contact's audience membership.
/// </summary>
public sealed record ContactSubscriberInfo
{
    /// <summary>
    /// Subscriber ID.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = null!;

    /// <summary>
    /// Whether the contact is subscribed to this audience.
    /// </summary>
    [JsonPropertyName("subscribed")]
    public bool Subscribed { get; init; }

    /// <summary>
    /// Timestamp when the contact subscribed.
    /// </summary>
    [JsonPropertyName("subscribed_at")]
    public DateTime? SubscribedAt { get; init; }

    /// <summary>
    /// Timestamp when the contact unsubscribed.
    /// </summary>
    [JsonPropertyName("unsubscribed_at")]
    public DateTime? UnsubscribedAt { get; init; }

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

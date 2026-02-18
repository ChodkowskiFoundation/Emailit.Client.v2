using System.Text.Json.Serialization;

namespace Emailit.Client.Models.Subscribers;

/// <summary>
/// Response model for subscriber operations.
/// </summary>
public sealed record SubscriberResponse
{
    /// <summary>
    /// Object type identifier.
    /// </summary>
    [JsonPropertyName("object")]
    public string Object { get; init; } = "subscriber";

    /// <summary>
    /// Unique subscriber identifier (prefixed with sub_).
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = null!;

    /// <summary>
    /// Subscriber's email address.
    /// </summary>
    [JsonPropertyName("email")]
    public string Email { get; init; } = null!;

    /// <summary>
    /// Subscriber's first name.
    /// </summary>
    [JsonPropertyName("first_name")]
    public string? FirstName { get; init; }

    /// <summary>
    /// Subscriber's last name.
    /// </summary>
    [JsonPropertyName("last_name")]
    public string? LastName { get; init; }

    /// <summary>
    /// Audience ID this subscriber belongs to.
    /// </summary>
    [JsonPropertyName("audience_id")]
    public string? AudienceId { get; init; }

    /// <summary>
    /// Contact ID linked to this subscriber.
    /// </summary>
    [JsonPropertyName("contact_id")]
    public string? ContactId { get; init; }

    /// <summary>
    /// Whether the subscriber is currently subscribed.
    /// </summary>
    [JsonPropertyName("subscribed")]
    public bool? Subscribed { get; init; }

    /// <summary>
    /// When the subscriber subscribed.
    /// </summary>
    [JsonPropertyName("subscribed_at")]
    public DateTime? SubscribedAt { get; init; }

    /// <summary>
    /// When the subscriber unsubscribed.
    /// </summary>
    [JsonPropertyName("unsubscribed_at")]
    public DateTime? UnsubscribedAt { get; init; }

    /// <summary>
    /// Custom fields for the subscriber.
    /// </summary>
    [JsonPropertyName("custom_fields")]
    public Dictionary<string, object>? CustomFields { get; init; }

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

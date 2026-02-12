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
    /// Subscription status (subscribed, unsubscribed, bounced).
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; init; }

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

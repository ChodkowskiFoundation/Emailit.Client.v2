using System.Text.Json.Serialization;

namespace Emailit.Client.Models.Audiences;

/// <summary>
/// Response model for audience operations.
/// </summary>
public sealed record AudienceResponse
{
    /// <summary>
    /// Object type identifier.
    /// </summary>
    [JsonPropertyName("object")]
    public string Object { get; init; } = "audience";

    /// <summary>
    /// Unique audience identifier (prefixed with aud_).
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = null!;

    /// <summary>
    /// Audience name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = null!;

    /// <summary>
    /// Unique token for form subscriptions.
    /// </summary>
    [JsonPropertyName("token")]
    public string? Token { get; init; }

    /// <summary>
    /// Number of subscribers in this audience.
    /// </summary>
    [JsonPropertyName("subscriber_count")]
    public int? SubscriberCount { get; init; }

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

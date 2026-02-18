using System.Text.Json.Serialization;

namespace Emailit.Client.Models.Contacts;

/// <summary>
/// Audience membership information for a contact.
/// </summary>
public sealed record ContactAudienceInfo
{
    /// <summary>
    /// Audience ID.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = null!;

    /// <summary>
    /// Audience name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = null!;

    /// <summary>
    /// Subscriber details within this audience.
    /// </summary>
    [JsonPropertyName("subscriber")]
    public ContactSubscriberInfo? Subscriber { get; init; }
}

using System.Text.Json.Serialization;

namespace Emailit.Client.Models.Subscribers;

/// <summary>
/// Request to add a subscriber to an audience.
/// </summary>
public sealed class AddSubscriberRequest
{
    /// <summary>
    /// Subscriber's email address.
    /// </summary>
    [JsonPropertyName("email")]
    public required string Email { get; init; }

    /// <summary>
    /// Subscriber's first name.
    /// </summary>
    [JsonPropertyName("first_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? FirstName { get; init; }

    /// <summary>
    /// Subscriber's last name.
    /// </summary>
    [JsonPropertyName("last_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? LastName { get; init; }

    /// <summary>
    /// Custom fields for the subscriber.
    /// </summary>
    [JsonPropertyName("custom_fields")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, object>? CustomFields { get; init; }
}

using System.Text.Json.Serialization;

namespace Emailit.Client.Models.Subscribers;

/// <summary>
/// Request to update a subscriber.
/// </summary>
public sealed class UpdateSubscriberRequest
{
    /// <summary>
    /// New email address.
    /// </summary>
    [JsonPropertyName("email")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Email { get; init; }

    /// <summary>
    /// First name.
    /// </summary>
    [JsonPropertyName("first_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? FirstName { get; init; }

    /// <summary>
    /// Last name.
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

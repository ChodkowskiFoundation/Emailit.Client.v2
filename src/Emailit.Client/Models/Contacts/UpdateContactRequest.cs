using System.Text.Json.Serialization;

namespace Emailit.Client.Models.Contacts;

/// <summary>
/// Request model for updating a contact.
/// </summary>
public sealed class UpdateContactRequest
{
    /// <summary>
    /// New email address.
    /// </summary>
    [JsonPropertyName("email")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Email { get; init; }

    /// <summary>
    /// Updated first name.
    /// </summary>
    [JsonPropertyName("first_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? FirstName { get; init; }

    /// <summary>
    /// Updated last name.
    /// </summary>
    [JsonPropertyName("last_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? LastName { get; init; }

    /// <summary>
    /// Replace custom fields.
    /// </summary>
    [JsonPropertyName("custom_fields")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, object>? CustomFields { get; init; }

    /// <summary>
    /// Global unsubscribe status.
    /// </summary>
    [JsonPropertyName("unsubscribed")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Unsubscribed { get; init; }
}

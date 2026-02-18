using System.Text.Json.Serialization;

namespace Emailit.Client.Models.Contacts;

/// <summary>
/// Request model for creating a contact.
/// </summary>
public sealed class CreateContactRequest
{
    /// <summary>
    /// Contact email address.
    /// </summary>
    [JsonPropertyName("email")]
    public required string Email { get; init; }

    /// <summary>
    /// Contact first name.
    /// </summary>
    [JsonPropertyName("first_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? FirstName { get; init; }

    /// <summary>
    /// Contact last name.
    /// </summary>
    [JsonPropertyName("last_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? LastName { get; init; }

    /// <summary>
    /// Custom fields as key-value pairs.
    /// </summary>
    [JsonPropertyName("custom_fields")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, object>? CustomFields { get; init; }

    /// <summary>
    /// Array of audience IDs to subscribe the contact to.
    /// </summary>
    [JsonPropertyName("audiences")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyList<string>? Audiences { get; init; }

    /// <summary>
    /// Global unsubscribe status.
    /// </summary>
    [JsonPropertyName("unsubscribed")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Unsubscribed { get; init; }
}

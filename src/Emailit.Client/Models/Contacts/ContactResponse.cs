using System.Text.Json.Serialization;

namespace Emailit.Client.Models.Contacts;

/// <summary>
/// Response model for contact operations.
/// </summary>
public sealed record ContactResponse
{
    /// <summary>
    /// Object type identifier.
    /// </summary>
    [JsonPropertyName("object")]
    public string Object { get; init; } = "contact";

    /// <summary>
    /// Unique contact identifier (prefixed with con_).
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = null!;

    /// <summary>
    /// Contact email address.
    /// </summary>
    [JsonPropertyName("email")]
    public string Email { get; init; } = null!;

    /// <summary>
    /// Contact first name.
    /// </summary>
    [JsonPropertyName("first_name")]
    public string? FirstName { get; init; }

    /// <summary>
    /// Contact last name.
    /// </summary>
    [JsonPropertyName("last_name")]
    public string? LastName { get; init; }

    /// <summary>
    /// Custom fields.
    /// </summary>
    [JsonPropertyName("custom_fields")]
    public Dictionary<string, object>? CustomFields { get; init; }

    /// <summary>
    /// Global unsubscribe status.
    /// </summary>
    [JsonPropertyName("unsubscribed")]
    public bool Unsubscribed { get; init; }

    /// <summary>
    /// Audience memberships with subscriber details.
    /// </summary>
    [JsonPropertyName("audiences")]
    public List<ContactAudienceInfo>? Audiences { get; init; }

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

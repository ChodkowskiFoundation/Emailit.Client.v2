using System.Text.Json.Serialization;

namespace Emailit.Client.Models.ApiKeys;

/// <summary>
/// Response model for API key operations.
/// </summary>
public sealed record ApiKeyResponse
{
    /// <summary>
    /// Object type identifier.
    /// </summary>
    [JsonPropertyName("object")]
    public string Object { get; init; } = "api_key";

    /// <summary>
    /// Unique API key identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = null!;

    /// <summary>
    /// API key name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = null!;

    /// <summary>
    /// Key scope (full or sending).
    /// </summary>
    [JsonPropertyName("scope")]
    public string? Scope { get; init; }

    /// <summary>
    /// Associated sending domain ID.
    /// </summary>
    [JsonPropertyName("sending_domain_id")]
    public string? SendingDomainId { get; init; }

    /// <summary>
    /// The actual API key value (only returned on creation).
    /// </summary>
    [JsonPropertyName("key")]
    public string? Key { get; init; }

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

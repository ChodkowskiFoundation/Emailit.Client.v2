using System.Text.Json.Serialization;

namespace Emailit.Client.Models.ApiKeys;

/// <summary>
/// Response for API key deletion.
/// </summary>
public sealed record DeleteApiKeyResponse
{
    /// <summary>
    /// Object type identifier.
    /// </summary>
    [JsonPropertyName("object")]
    public string Object { get; init; } = "api_key";

    /// <summary>
    /// API key ID that was deleted.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = null!;

    /// <summary>
    /// Whether the deletion was successful.
    /// </summary>
    [JsonPropertyName("deleted")]
    public bool Deleted { get; init; }
}

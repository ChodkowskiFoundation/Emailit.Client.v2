using System.Text.Json.Serialization;

namespace Emailit.Client.Models.ApiKeys;

/// <summary>
/// Request to update an API key.
/// </summary>
public sealed class UpdateApiKeyRequest
{
    /// <summary>
    /// New name for the API key.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }
}

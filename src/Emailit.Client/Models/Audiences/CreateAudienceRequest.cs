using System.Text.Json.Serialization;

namespace Emailit.Client.Models.Audiences;

/// <summary>
/// Request to create a new audience.
/// </summary>
public sealed class CreateAudienceRequest
{
    /// <summary>
    /// Audience name.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }
}

using System.Text.Json.Serialization;

namespace Emailit.Client.Models.Audiences;

/// <summary>
/// Request to update an audience.
/// </summary>
public sealed class UpdateAudienceRequest
{
    /// <summary>
    /// New audience name.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }
}

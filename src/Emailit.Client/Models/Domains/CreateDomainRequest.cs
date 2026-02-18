using System.Text.Json.Serialization;

namespace Emailit.Client.Models.Domains;

/// <summary>
/// Request to create a new sending domain.
/// </summary>
public sealed class CreateDomainRequest
{
    /// <summary>
    /// Domain name (e.g., "mail.yourdomain.com").
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// Enable email load/open tracking.
    /// </summary>
    [JsonPropertyName("track_loads")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? TrackLoads { get; init; }

    /// <summary>
    /// Enable click tracking.
    /// </summary>
    [JsonPropertyName("track_clicks")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? TrackClicks { get; init; }
}

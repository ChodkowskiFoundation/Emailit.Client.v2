using System.Text.Json.Serialization;

namespace Emailit.Client.Models.Domains;

/// <summary>
/// Request to update a domain's settings.
/// </summary>
public sealed class UpdateDomainRequest
{
    /// <summary>
    /// Default sender email address.
    /// </summary>
    [JsonPropertyName("from_email")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? FromEmail { get; init; }

    /// <summary>
    /// Enable open tracking.
    /// </summary>
    [JsonPropertyName("track_opens")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? TrackOpens { get; init; }

    /// <summary>
    /// Enable click tracking.
    /// </summary>
    [JsonPropertyName("track_clicks")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? TrackClicks { get; init; }
}

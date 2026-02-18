using System.Text.Json.Serialization;

namespace Emailit.Client.Models.Domains;

/// <summary>
/// Request to update a domain's settings. Sent via PATCH.
/// </summary>
public sealed class UpdateDomainRequest
{
    /// <summary>
    /// Enable load/open tracking.
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

    /// <summary>
    /// Custom subdomain for tracking CNAME (default: "go").
    /// </summary>
    [JsonPropertyName("tracking_key")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? TrackingKey { get; init; }

    /// <summary>
    /// Custom subdomain for inbound CNAME (default: "inbound").
    /// </summary>
    [JsonPropertyName("inbound_key")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? InboundKey { get; init; }
}

using System.Text.Json.Serialization;

namespace Emailit.Client.Models.Domains;

/// <summary>
/// Response model for domain operations.
/// </summary>
public sealed record DomainResponse
{
    /// <summary>
    /// Object type identifier.
    /// </summary>
    [JsonPropertyName("object")]
    public string Object { get; init; } = "domain";

    /// <summary>
    /// Unique domain identifier (prefixed with dom_).
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = null!;

    /// <summary>
    /// Domain name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = null!;

    /// <summary>
    /// Default sender email address.
    /// </summary>
    [JsonPropertyName("from_email")]
    public string? FromEmail { get; init; }

    /// <summary>
    /// Domain verification status.
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; init; } = null!;

    /// <summary>
    /// DNS records required for verification.
    /// </summary>
    [JsonPropertyName("dns_records")]
    public List<DnsRecord>? DnsRecords { get; init; }

    /// <summary>
    /// Whether open tracking is enabled.
    /// </summary>
    [JsonPropertyName("track_opens")]
    public bool? TrackOpens { get; init; }

    /// <summary>
    /// Whether click tracking is enabled.
    /// </summary>
    [JsonPropertyName("track_clicks")]
    public bool? TrackClicks { get; init; }

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

/// <summary>
/// DNS record for domain verification.
/// </summary>
public sealed record DnsRecord
{
    /// <summary>
    /// Record type (e.g., "TXT", "CNAME").
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; init; } = null!;

    /// <summary>
    /// Record name/host.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = null!;

    /// <summary>
    /// Record value.
    /// </summary>
    [JsonPropertyName("value")]
    public string Value { get; init; } = null!;

    /// <summary>
    /// TTL in seconds.
    /// </summary>
    [JsonPropertyName("ttl")]
    public int? Ttl { get; init; }

    /// <summary>
    /// Verification status for this record.
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; init; }
}

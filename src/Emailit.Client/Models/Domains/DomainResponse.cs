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
    /// Unique domain identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = null!;

    /// <summary>
    /// Domain UUID.
    /// </summary>
    [JsonPropertyName("uuid")]
    public string? Uuid { get; init; }

    /// <summary>
    /// Domain name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = null!;

    /// <summary>
    /// Verification token.
    /// </summary>
    [JsonPropertyName("verification_token")]
    public string? VerificationToken { get; init; }

    /// <summary>
    /// Verification method.
    /// </summary>
    [JsonPropertyName("verification_method")]
    public string? VerificationMethod { get; init; }

    /// <summary>
    /// SPF verification status (ok, failed, pending, missing).
    /// </summary>
    [JsonPropertyName("spf_status")]
    public string? SpfStatus { get; init; }

    /// <summary>
    /// DKIM verification status (ok, failed, pending, missing).
    /// </summary>
    [JsonPropertyName("dkim_status")]
    public string? DkimStatus { get; init; }

    /// <summary>
    /// MX verification status (ok, failed, pending, missing).
    /// </summary>
    [JsonPropertyName("mx_status")]
    public string? MxStatus { get; init; }

    /// <summary>
    /// DMARC verification status (ok, failed, pending, missing).
    /// </summary>
    [JsonPropertyName("dmarc_status")]
    public string? DmarcStatus { get; init; }

    /// <summary>
    /// DKIM identifier string.
    /// </summary>
    [JsonPropertyName("dkim_identifier_string")]
    public string? DkimIdentifierString { get; init; }

    /// <summary>
    /// DNS records required for verification.
    /// </summary>
    [JsonPropertyName("dns_records")]
    public List<DnsRecordResponse>? DnsRecords { get; init; }

    /// <summary>
    /// Whether load/open tracking is enabled.
    /// </summary>
    [JsonPropertyName("track_loads")]
    public bool? TrackLoads { get; init; }

    /// <summary>
    /// Whether click tracking is enabled.
    /// </summary>
    [JsonPropertyName("track_clicks")]
    public bool? TrackClicks { get; init; }

    /// <summary>
    /// Timestamp when the domain was verified.
    /// </summary>
    [JsonPropertyName("verified_at")]
    public DateTime? VerifiedAt { get; init; }

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

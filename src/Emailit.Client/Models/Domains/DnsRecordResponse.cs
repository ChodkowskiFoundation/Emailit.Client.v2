using System.Text.Json.Serialization;

namespace Emailit.Client.Models.Domains;

/// <summary>
/// DNS record for domain verification.
/// </summary>
public sealed record DnsRecordResponse
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

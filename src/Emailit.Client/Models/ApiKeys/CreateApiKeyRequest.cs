using System.Text.Json.Serialization;

namespace Emailit.Client.Models.ApiKeys;

/// <summary>
/// Request to create a new API key.
/// </summary>
public sealed class CreateApiKeyRequest
{
    /// <summary>
    /// Name/identifier for the API key.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// Key scope: "full" (all operations) or "sending" (send emails only).
    /// </summary>
    [JsonPropertyName("scope")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Scope { get; init; }

    /// <summary>
    /// Restrict key to specific sending domain ID.
    /// </summary>
    [JsonPropertyName("sending_domain_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? SendingDomainId { get; init; }
}

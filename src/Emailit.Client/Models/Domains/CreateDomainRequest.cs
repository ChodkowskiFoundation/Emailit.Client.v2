using System.Text.Json.Serialization;

namespace Emailit.Client.Models.Domains;

/// <summary>
/// Request to create a new sending domain.
/// </summary>
public sealed class CreateDomainRequest
{
    /// <summary>
    /// Domain name (e.g., "example.com").
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// Default sender email address for this domain.
    /// </summary>
    [JsonPropertyName("from_email")]
    public required string FromEmail { get; init; }
}

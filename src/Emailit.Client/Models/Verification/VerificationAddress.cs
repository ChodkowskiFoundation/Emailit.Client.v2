using System.Text.Json.Serialization;

namespace Emailit.Client.Models.Verification;

/// <summary>
/// Parsed email address components.
/// </summary>
public sealed record VerificationAddress
{
    /// <summary>
    /// Local part of the email address (before @).
    /// </summary>
    [JsonPropertyName("mailbox")]
    public string? Mailbox { get; init; }

    /// <summary>
    /// Full domain of the email address.
    /// </summary>
    [JsonPropertyName("domain")]
    public string? Domain { get; init; }

    /// <summary>
    /// Domain suffix (e.g., .com, .co.uk).
    /// </summary>
    [JsonPropertyName("suffix")]
    public string? Suffix { get; init; }

    /// <summary>
    /// Root domain (without suffix).
    /// </summary>
    [JsonPropertyName("root")]
    public string? Root { get; init; }
}

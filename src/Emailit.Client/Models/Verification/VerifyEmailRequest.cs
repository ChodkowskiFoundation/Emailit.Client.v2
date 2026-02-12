using System.Text.Json.Serialization;

namespace Emailit.Client.Models.Verification;

/// <summary>
/// Request to verify a single email address.
/// </summary>
public sealed class VerifyEmailRequest
{
    /// <summary>
    /// Email address to verify.
    /// </summary>
    [JsonPropertyName("email")]
    public required string Email { get; init; }
}

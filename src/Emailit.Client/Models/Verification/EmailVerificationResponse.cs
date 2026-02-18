using System.Text.Json.Serialization;

namespace Emailit.Client.Models.Verification;

/// <summary>
/// Response model for email verification.
/// </summary>
public sealed record EmailVerificationResponse
{
    /// <summary>
    /// Object type identifier.
    /// </summary>
    [JsonPropertyName("object")]
    public string Object { get; init; } = "email_verification";

    /// <summary>
    /// Unique verification identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = null!;

    /// <summary>
    /// Email address that was verified.
    /// </summary>
    [JsonPropertyName("email")]
    public string Email { get; init; } = null!;

    /// <summary>
    /// Verification status (completed, pending, etc.).
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; init; }

    /// <summary>
    /// Verification result: "safe", "invalid", "risky", "unknown".
    /// </summary>
    [JsonPropertyName("result")]
    public string Result { get; init; } = null!;

    /// <summary>
    /// Verification score (0-100).
    /// </summary>
    [JsonPropertyName("score")]
    public int? Score { get; init; }

    /// <summary>
    /// Risk level: "low", "medium", "high".
    /// </summary>
    [JsonPropertyName("risk")]
    public string? Risk { get; init; }

    /// <summary>
    /// Verification mode used.
    /// </summary>
    [JsonPropertyName("mode")]
    public string? Mode { get; init; }

    /// <summary>
    /// Detailed verification check results.
    /// </summary>
    [JsonPropertyName("checks")]
    public VerificationChecks? Checks { get; init; }

    /// <summary>
    /// Parsed email address components.
    /// </summary>
    [JsonPropertyName("address")]
    public VerificationAddress? Address { get; init; }

    /// <summary>
    /// Suggested email correction (e.g., for typos).
    /// </summary>
    [JsonPropertyName("did_you_mean")]
    public string? DidYouMean { get; init; }

    /// <summary>
    /// MX records for the domain.
    /// </summary>
    [JsonPropertyName("mx_records")]
    public List<MxRecordInfo>? MxRecords { get; init; }
}

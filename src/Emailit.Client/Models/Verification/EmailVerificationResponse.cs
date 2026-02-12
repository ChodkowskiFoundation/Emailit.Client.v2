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
    /// Verification result: "valid", "invalid", "risky", "unknown".
    /// </summary>
    [JsonPropertyName("result")]
    public string Result { get; init; } = null!;

    /// <summary>
    /// Risk score (0-100, where 0 is safest).
    /// </summary>
    [JsonPropertyName("risk_score")]
    public int? RiskScore { get; init; }

    /// <summary>
    /// Whether the email is deliverable.
    /// </summary>
    [JsonPropertyName("is_deliverable")]
    public bool? IsDeliverable { get; init; }

    /// <summary>
    /// Whether the domain is disposable.
    /// </summary>
    [JsonPropertyName("is_disposable")]
    public bool? IsDisposable { get; init; }

    /// <summary>
    /// Whether the email is a role account (e.g., info@, support@).
    /// </summary>
    [JsonPropertyName("is_role_account")]
    public bool? IsRoleAccount { get; init; }

    /// <summary>
    /// Whether the email is a free provider (e.g., gmail.com).
    /// </summary>
    [JsonPropertyName("is_free_provider")]
    public bool? IsFreeProvider { get; init; }

    /// <summary>
    /// Whether MX records exist for the domain.
    /// </summary>
    [JsonPropertyName("has_mx_records")]
    public bool? HasMxRecords { get; init; }

    /// <summary>
    /// SMTP provider name.
    /// </summary>
    [JsonPropertyName("smtp_provider")]
    public string? SmtpProvider { get; init; }

    /// <summary>
    /// Verification timestamp.
    /// </summary>
    [JsonPropertyName("verified_at")]
    public DateTime? VerifiedAt { get; init; }
}

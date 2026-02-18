using System.Text.Json.Serialization;

namespace Emailit.Client.Models.Verification;

/// <summary>
/// Detailed verification check results.
/// </summary>
public sealed record VerificationChecks
{
    /// <summary>
    /// Whether the email has valid syntax.
    /// </summary>
    [JsonPropertyName("valid_syntax")]
    public bool? ValidSyntax { get; init; }

    /// <summary>
    /// Whether the domain is disposable.
    /// </summary>
    [JsonPropertyName("disposable")]
    public bool? Disposable { get; init; }

    /// <summary>
    /// Whether the email is a role account (e.g., info@, support@).
    /// </summary>
    [JsonPropertyName("role_account")]
    public bool? RoleAccount { get; init; }

    /// <summary>
    /// Whether the inbox is full.
    /// </summary>
    [JsonPropertyName("inbox_full")]
    public bool? InboxFull { get; init; }

    /// <summary>
    /// Whether the email is deliverable.
    /// </summary>
    [JsonPropertyName("deliverable")]
    public bool? Deliverable { get; init; }

    /// <summary>
    /// Whether the account is disabled.
    /// </summary>
    [JsonPropertyName("disabled")]
    public bool? Disabled { get; init; }

    /// <summary>
    /// Whether the email is from a free provider (e.g., gmail.com).
    /// </summary>
    [JsonPropertyName("free_email")]
    public bool? FreeEmail { get; init; }

    /// <summary>
    /// Whether the email appears to be gibberish.
    /// </summary>
    [JsonPropertyName("gibberish")]
    public bool? Gibberish { get; init; }

    /// <summary>
    /// Whether the domain is a catch-all.
    /// </summary>
    [JsonPropertyName("catch_all")]
    public bool? CatchAll { get; init; }

    /// <summary>
    /// Whether SMTP connection was successful.
    /// </summary>
    [JsonPropertyName("smtp_connect")]
    public bool? SmtpConnect { get; init; }

    /// <summary>
    /// Whether MX records exist for the domain.
    /// </summary>
    [JsonPropertyName("has_mx_records")]
    public bool? HasMxRecords { get; init; }

    /// <summary>
    /// Domain age in days.
    /// </summary>
    [JsonPropertyName("domain_age")]
    public int? DomainAge { get; init; }
}

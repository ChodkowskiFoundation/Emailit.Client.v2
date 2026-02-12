using System.Text.Json.Serialization;

namespace Emailit.Client.Models.Verification;

/// <summary>
/// Response model for verification list operations.
/// </summary>
public sealed record VerificationListResponse
{
    /// <summary>
    /// Object type identifier.
    /// </summary>
    [JsonPropertyName("object")]
    public string Object { get; init; } = "email_verification_list";

    /// <summary>
    /// Unique list identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = null!;

    /// <summary>
    /// List name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    /// Processing status: "pending", "processing", "completed", "failed".
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; init; } = null!;

    /// <summary>
    /// Total number of emails in the list.
    /// </summary>
    [JsonPropertyName("total_count")]
    public int? TotalCount { get; init; }

    /// <summary>
    /// Number of emails processed so far.
    /// </summary>
    [JsonPropertyName("processed_count")]
    public int? ProcessedCount { get; init; }

    /// <summary>
    /// Number of valid emails.
    /// </summary>
    [JsonPropertyName("valid_count")]
    public int? ValidCount { get; init; }

    /// <summary>
    /// Number of invalid emails.
    /// </summary>
    [JsonPropertyName("invalid_count")]
    public int? InvalidCount { get; init; }

    /// <summary>
    /// Number of risky emails.
    /// </summary>
    [JsonPropertyName("risky_count")]
    public int? RiskyCount { get; init; }

    /// <summary>
    /// Number of unknown emails.
    /// </summary>
    [JsonPropertyName("unknown_count")]
    public int? UnknownCount { get; init; }

    /// <summary>
    /// Creation timestamp.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// Completion timestamp.
    /// </summary>
    [JsonPropertyName("completed_at")]
    public DateTime? CompletedAt { get; init; }
}

using System.Text.Json.Serialization;

namespace Emailit.Client.Models.Emails;

/// <summary>
/// Response model for email operations.
/// </summary>
public sealed record EmailResponse
{
    /// <summary>
    /// Object type identifier.
    /// </summary>
    [JsonPropertyName("object")]
    public string Object { get; init; } = "email";

    /// <summary>
    /// Unique email identifier (prefixed with em_).
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = null!;

    /// <summary>
    /// Current email status, one of following values: "pending", "scheduled", "sent", "delivered", "bounced", "canceled".
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; init; } = null!;

    /// <summary>
    /// Sender email address.
    /// </summary>
    [JsonPropertyName("from")]
    public string? From { get; init; }

    /// <summary>
    /// Recipient email addresses.
    /// </summary>
    [JsonPropertyName("to")]
    public List<string>? To { get; init; }

    /// <summary>
    /// Email subject.
    /// </summary>
    [JsonPropertyName("subject")]
    public string? Subject { get; init; }

    /// <summary>
    /// Creation timestamp.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// Timestamp when the email was sent.
    /// </summary>
    [JsonPropertyName("sent_at")]
    public DateTime? SentAt { get; init; }

    /// <summary>
    /// Timestamp when the email was delivered.
    /// </summary>
    [JsonPropertyName("delivered_at")]
    public DateTime? DeliveredAt { get; init; }

    /// <summary>
    /// Timestamp when the email bounced.
    /// </summary>
    [JsonPropertyName("bounced_at")]
    public DateTime? BouncedAt { get; init; }

    /// <summary>
    /// Bounce type (hard, soft).
    /// </summary>
    [JsonPropertyName("bounce_type")]
    public string? BounceType { get; init; }

    /// <summary>
    /// Bounce reason message.
    /// </summary>
    [JsonPropertyName("bounce_reason")]
    public string? BounceReason { get; init; }

    /// <summary>
    /// Scheduled send time.
    /// </summary>
    [JsonPropertyName("scheduled_at")]
    public DateTime? ScheduledAt { get; init; }

    /// <summary>
    /// Custom metadata.
    /// </summary>
    [JsonPropertyName("metadata")]
    public Dictionary<string, string>? Metadata { get; init; }

    /// <summary>
    /// Last update timestamp.
    /// </summary>
    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; init; }

    /// <summary>
    /// Rate limit information from response headers (not from JSON).
    /// </summary>
    [JsonIgnore]
    public RateLimitInfo? RateLimitInfo { get; init; }
}

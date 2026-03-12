using System.Text.Json.Serialization;

namespace Emailit.Client.Models.Emails;

/// <summary>
/// Response model for email metadata (without body content).
/// </summary>
public sealed record EmailMetaResponse
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
    /// Email type: "inbound" or "outbound".
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; init; }

    /// <summary>
    /// Email token.
    /// </summary>
    [JsonPropertyName("token")]
    public string? Token { get; init; }

    /// <summary>
    /// Message-ID header value.
    /// </summary>
    [JsonPropertyName("message_id")]
    public string? MessageId { get; init; }

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
    /// Current email status.
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; init; } = null!;

    /// <summary>
    /// Email size in bytes.
    /// </summary>
    [JsonPropertyName("size")]
    public int? Size { get; init; }

    /// <summary>
    /// Tracking configuration.
    /// </summary>
    [JsonPropertyName("tracking")]
    public EmailTrackingOptions? Tracking { get; init; }

    /// <summary>
    /// Custom email headers.
    /// </summary>
    [JsonPropertyName("headers")]
    public Dictionary<string, string>? Headers { get; init; }

    /// <summary>
    /// Metadata key-value pairs.
    /// </summary>
    [JsonPropertyName("meta")]
    public Dictionary<string, string>? Meta { get; init; }

    /// <summary>
    /// Attachment metadata (without content).
    /// </summary>
    [JsonPropertyName("attachments")]
    public List<EmailAttachmentInfo>? Attachments { get; init; }

    /// <summary>
    /// Scheduled send time.
    /// </summary>
    [JsonPropertyName("scheduled_at")]
    public DateTime? ScheduledAt { get; init; }

    /// <summary>
    /// Creation timestamp.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// Last update timestamp.
    /// </summary>
    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; init; }

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
}

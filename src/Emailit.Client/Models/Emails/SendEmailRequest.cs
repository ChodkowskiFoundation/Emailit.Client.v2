using System.Text.Json.Serialization;

namespace Emailit.Client.Models.Emails;

/// <summary>
/// Request model for sending an email via Emailit API.
/// </summary>
public sealed class SendEmailRequest
{
    /// <summary>
    /// Sender email address. Format: "Name &lt;email@domain.com&gt;" or "email@domain.com"
    /// </summary>
    [JsonPropertyName("from")]
    public required string From { get; init; }

    /// <summary>
    /// List of recipient email addresses (max 50 combined with CC and BCC).
    /// </summary>
    [JsonPropertyName("to")]
    public required IReadOnlyList<string> To { get; init; }

    /// <summary>
    /// List of CC recipient email addresses.
    /// </summary>
    [JsonPropertyName("cc")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyList<string>? Cc { get; init; }

    /// <summary>
    /// List of BCC recipient email addresses.
    /// </summary>
    [JsonPropertyName("bcc")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyList<string>? Bcc { get; init; }

    /// <summary>
    /// Reply-to email addresses.
    /// </summary>
    [JsonPropertyName("reply_to")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyList<string>? ReplyTo { get; init; }

    /// <summary>
    /// Email subject line.
    /// </summary>
    [JsonPropertyName("subject")]
    public required string Subject { get; init; }

    /// <summary>
    /// HTML body content. Required if template_id is not provided.
    /// </summary>
    [JsonPropertyName("html")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Html { get; init; }

    /// <summary>
    /// Plain text body content.
    /// </summary>
    [JsonPropertyName("text")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Text { get; init; }

    /// <summary>
    /// Template ID to use for the email body.
    /// </summary>
    [JsonPropertyName("template")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? TemplateId { get; init; }

    /// <summary>
    /// Variables for template substitution.
    /// </summary>
    [JsonPropertyName("variables")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, object>? Variables { get; init; }

    /// <summary>
    /// Email attachments.
    /// </summary>
    [JsonPropertyName("attachments")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyList<EmailAttachment>? Attachments { get; init; }

    /// <summary>
    /// Scheduled send time (ISO 8601, Unix timestamp, or natural language).
    /// </summary>
    [JsonPropertyName("scheduled_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ScheduledAt { get; init; }

    /// <summary>
    /// Enable open tracking.
    /// </summary>
    [JsonPropertyName("track_opens")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? TrackOpens { get; init; }

    /// <summary>
    /// Enable click tracking.
    /// </summary>
    [JsonPropertyName("track_clicks")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? TrackClicks { get; init; }

    /// <summary>
    /// Custom metadata key-value pairs.
    /// </summary>
    [JsonPropertyName("metadata")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, string>? Metadata { get; init; }
}

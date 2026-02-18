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
    /// Email subject line. Required unless a template provides it.
    /// </summary>
    [JsonPropertyName("subject")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Subject { get; init; }

    /// <summary>
    /// HTML body content. Required if template is not provided.
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
    /// Template alias or ID (tem_xxx) to use for the email body.
    /// </summary>
    [JsonPropertyName("template")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? TemplateId { get; init; }

    /// <summary>
    /// Variables for template substitution using {{variable}} syntax.
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
    /// Custom email headers as key-value pairs.
    /// </summary>
    [JsonPropertyName("headers")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, string>? Headers { get; init; }

    /// <summary>
    /// Metadata as key-value string pairs.
    /// </summary>
    [JsonPropertyName("meta")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, string>? Meta { get; init; }

    /// <summary>
    /// Scheduled send time (ISO 8601, Unix timestamp, or natural language).
    /// </summary>
    [JsonPropertyName("scheduled_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ScheduledAt { get; init; }

    /// <summary>
    /// Tracking configuration with loads and clicks options.
    /// </summary>
    [JsonPropertyName("tracking")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public EmailTrackingOptions? Tracking { get; init; }
}

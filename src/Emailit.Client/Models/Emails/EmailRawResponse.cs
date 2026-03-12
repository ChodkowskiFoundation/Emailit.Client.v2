using System.Text.Json.Serialization;

namespace Emailit.Client.Models.Emails;

/// <summary>
/// Response model for raw MIME email message.
/// </summary>
public sealed record EmailRawResponse
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
    /// Full raw MIME message string.
    /// </summary>
    [JsonPropertyName("raw")]
    public string Raw { get; init; } = null!;

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
    /// Message-ID header value.
    /// </summary>
    [JsonPropertyName("message_id")]
    public string? MessageId { get; init; }

    /// <summary>
    /// Email size in bytes.
    /// </summary>
    [JsonPropertyName("size")]
    public int? Size { get; init; }

    /// <summary>
    /// Creation timestamp.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; init; }
}

using System.Text.Json.Serialization;

namespace Emailit.Client.Models.Emails;

/// <summary>
/// Response model for email attachments with base64-encoded content.
/// </summary>
[JsonConverter(typeof(EmailAttachmentsResponseJsonConverter))]
public sealed record EmailAttachmentsResponse
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
    public string? Id { get; init; }

    /// <summary>
    /// List of attachments with base64-encoded content.
    /// </summary>
    [JsonPropertyName("attachments")]
    public List<EmailAttachment> Attachments { get; init; } = [];
}

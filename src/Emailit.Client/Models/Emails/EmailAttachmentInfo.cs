using System.Text.Json.Serialization;

namespace Emailit.Client.Models.Emails;

/// <summary>
/// Attachment metadata without content payload.
/// </summary>
public sealed class EmailAttachmentInfo
{
    /// <summary>
    /// Filename with extension.
    /// </summary>
    [JsonPropertyName("filename")]
    public string Filename { get; init; } = null!;

    /// <summary>
    /// MIME content type (e.g., "application/pdf").
    /// </summary>
    [JsonPropertyName("content_type")]
    public string ContentType { get; init; } = null!;

    /// <summary>
    /// Attachment size in bytes.
    /// </summary>
    [JsonPropertyName("size")]
    public int? Size { get; init; }

    /// <summary>
    /// Content ID for inline images (cid:).
    /// </summary>
    [JsonPropertyName("content_id")]
    public string? ContentId { get; init; }
}

using System.Text.Json.Serialization;

namespace Emailit.Client.Models.Emails;

/// <summary>
/// Email attachment model for Emailit API.
/// </summary>
public sealed class EmailAttachment
{
    /// <summary>
    /// Filename with extension.
    /// </summary>
    [JsonPropertyName("filename")]
    public required string Filename { get; init; }

    /// <summary>
    /// Base64-encoded content (for files ≤ 5MB).
    /// </summary>
    [JsonPropertyName("content")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Content { get; init; }

    /// <summary>
    /// URL to download the attachment (for files > 5MB, must be HTTPS with min 24h TTL).
    /// </summary>
    [JsonPropertyName("url")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Url { get; init; }

    /// <summary>
    /// MIME content type (e.g., "application/pdf").
    /// </summary>
    [JsonPropertyName("content_type")]
    public required string ContentType { get; init; }

    /// <summary>
    /// Content ID for inline images (cid:).
    /// </summary>
    [JsonPropertyName("content_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ContentId { get; init; }

    /// <summary>
    /// Content encoding (default: base64).
    /// </summary>
    [JsonPropertyName("encoding")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Encoding { get; init; }
}

using System.Text.Json.Serialization;

namespace Emailit.Client.Models.Emails;

/// <summary>
/// Response model for email body content (text and HTML only).
/// </summary>
public sealed record EmailBodyResponse
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
    /// Plain text content.
    /// </summary>
    [JsonPropertyName("text")]
    public string? Text { get; init; }

    /// <summary>
    /// HTML content.
    /// </summary>
    [JsonPropertyName("html")]
    public string? Html { get; init; }
}

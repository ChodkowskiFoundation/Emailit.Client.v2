using System.Text.Json.Serialization;

namespace Emailit.Client.Models.Templates;

/// <summary>
/// Response model for template operations.
/// </summary>
public sealed record TemplateResponse
{
    /// <summary>
    /// Object type identifier.
    /// </summary>
    [JsonPropertyName("object")]
    public string Object { get; init; } = "template";

    /// <summary>
    /// Unique template identifier (prefixed with tpl_).
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = null!;

    /// <summary>
    /// Template name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = null!;

    /// <summary>
    /// Grouping alias.
    /// </summary>
    [JsonPropertyName("alias")]
    public string? Alias { get; init; }

    /// <summary>
    /// Sender in RFC format.
    /// </summary>
    [JsonPropertyName("from")]
    public string? From { get; init; }

    /// <summary>
    /// Email subject line.
    /// </summary>
    [JsonPropertyName("subject")]
    public string? Subject { get; init; }

    /// <summary>
    /// Reply-to address(es).
    /// </summary>
    [JsonPropertyName("reply_to")]
    public IReadOnlyList<string>? ReplyTo { get; init; }

    /// <summary>
    /// HTML content of the template.
    /// </summary>
    [JsonPropertyName("html")]
    public string? Html { get; init; }

    /// <summary>
    /// Plain text content of the template.
    /// </summary>
    [JsonPropertyName("text")]
    public string? Text { get; init; }

    /// <summary>
    /// Editor type: "html", "tiptap", or "dragit".
    /// </summary>
    [JsonPropertyName("editor")]
    public string? Editor { get; init; }

    /// <summary>
    /// When the template was published.
    /// </summary>
    [JsonPropertyName("published_at")]
    public DateTime? PublishedAt { get; init; }

    /// <summary>
    /// Preview URL for the template.
    /// </summary>
    [JsonPropertyName("preview_url")]
    public string? PreviewUrl { get; init; }

    /// <summary>
    /// Total number of versions for this template.
    /// </summary>
    [JsonPropertyName("total_versions")]
    public int? TotalVersions { get; init; }

    /// <summary>
    /// Template version history.
    /// </summary>
    [JsonPropertyName("versions")]
    public List<TemplateResponse>? Versions { get; init; }

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
}

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
    /// Email subject line.
    /// </summary>
    [JsonPropertyName("subject")]
    public string? Subject { get; init; }

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
    /// Template version number.
    /// </summary>
    [JsonPropertyName("version")]
    public int? Version { get; init; }

    /// <summary>
    /// Whether the template is published.
    /// </summary>
    [JsonPropertyName("published")]
    public bool? Published { get; init; }

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

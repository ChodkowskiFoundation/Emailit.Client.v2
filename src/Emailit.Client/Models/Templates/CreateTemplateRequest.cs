using System.Text.Json.Serialization;

namespace Emailit.Client.Models.Templates;

/// <summary>
/// Request to create a new email template.
/// </summary>
public sealed class CreateTemplateRequest
{
    /// <summary>
    /// Template name (max 191 chars).
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// Grouping alias. Lowercase letters, numbers, underscores, hyphens only (max 191 chars).
    /// </summary>
    [JsonPropertyName("alias")]
    public required string Alias { get; init; }

    /// <summary>
    /// Sender in RFC format (max 191 chars).
    /// </summary>
    [JsonPropertyName("from")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? From { get; init; }

    /// <summary>
    /// Email subject line (can contain variables, max 191 chars).
    /// </summary>
    [JsonPropertyName("subject")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Subject { get; init; }

    /// <summary>
    /// Reply-to address(es).
    /// </summary>
    [JsonPropertyName("reply_to")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyList<string>? ReplyTo { get; init; }

    /// <summary>
    /// HTML content of the template.
    /// </summary>
    [JsonPropertyName("html")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Html { get; init; }

    /// <summary>
    /// Plain text content of the template.
    /// </summary>
    [JsonPropertyName("text")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Text { get; init; }

    /// <summary>
    /// Editor type: "html" (default), "tiptap", or "dragit".
    /// </summary>
    [JsonPropertyName("editor")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Editor { get; init; }
}

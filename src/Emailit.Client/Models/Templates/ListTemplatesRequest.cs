namespace Emailit.Client.Models.Templates;

/// <summary>
/// Request parameters for listing templates. Used to build query parameters.
/// </summary>
public sealed class ListTemplatesRequest
{
    /// <summary>
    /// Page number (default 1).
    /// </summary>
    public int Page { get; init; } = 1;

    /// <summary>
    /// Items per page (default 25).
    /// </summary>
    public int PerPage { get; init; } = 25;

    /// <summary>
    /// Filter by template name.
    /// </summary>
    public string? FilterName { get; init; }

    /// <summary>
    /// Filter by template alias.
    /// </summary>
    public string? FilterAlias { get; init; }

    /// <summary>
    /// Filter by editor type (html, tiptap, dragit).
    /// </summary>
    public string? FilterEditor { get; init; }

    /// <summary>
    /// Sort field: name, alias, created_at, updated_at.
    /// </summary>
    public string? Sort { get; init; }

    /// <summary>
    /// Sort order: asc or desc.
    /// </summary>
    public string? Order { get; init; }
}

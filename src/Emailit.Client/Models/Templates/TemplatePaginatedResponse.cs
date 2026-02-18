using System.Text.Json.Serialization;

namespace Emailit.Client.Models.Templates;

/// <summary>
/// Paginated response for template listing. Uses a different pagination format than other endpoints.
/// </summary>
public sealed record TemplatePaginatedResponse
{
    /// <summary>
    /// List of templates.
    /// </summary>
    [JsonPropertyName("data")]
    public List<TemplateResponse> Data { get; init; } = [];

    /// <summary>
    /// Total number of records.
    /// </summary>
    [JsonPropertyName("total_records")]
    public int TotalRecords { get; init; }

    /// <summary>
    /// Items per page.
    /// </summary>
    [JsonPropertyName("per_page")]
    public int PerPage { get; init; }

    /// <summary>
    /// Current page number.
    /// </summary>
    [JsonPropertyName("current_page")]
    public int CurrentPage { get; init; }

    /// <summary>
    /// Total number of pages.
    /// </summary>
    [JsonPropertyName("total_pages")]
    public int TotalPages { get; init; }
}

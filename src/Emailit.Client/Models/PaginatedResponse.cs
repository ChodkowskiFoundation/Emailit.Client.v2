using System.Text.Json.Serialization;

namespace Emailit.Client.Models;

/// <summary>
/// Base class for paginated API responses.
/// </summary>
/// <typeparam name="T">Type of items in the response.</typeparam>
public sealed class PaginatedResponse<T>
{
    /// <summary>
    /// The data items.
    /// </summary>
    [JsonPropertyName("data")]
    public List<T> Data { get; init; } = [];

    /// <summary>
    /// URL for the next page of results.
    /// </summary>
    [JsonPropertyName("next_page_url")]
    public string? NextPageUrl { get; init; }

    /// <summary>
    /// URL for the previous page of results.
    /// </summary>
    [JsonPropertyName("previous_page_url")]
    public string? PreviousPageUrl { get; init; }

    /// <summary>
    /// Whether there are more pages available.
    /// </summary>
    [JsonIgnore]
    public bool HasNextPage => !string.IsNullOrEmpty(NextPageUrl);

    /// <summary>
    /// Whether there is a previous page.
    /// </summary>
    [JsonIgnore]
    public bool HasPreviousPage => !string.IsNullOrEmpty(PreviousPageUrl);
}

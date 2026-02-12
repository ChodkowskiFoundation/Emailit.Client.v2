using System.Text.Json.Serialization;

namespace Emailit.Client.Models;

/// <summary>
/// Response model for cursor-based pagination (used by GET /v2/emails).
/// </summary>
/// <typeparam name="T">Type of items in the response.</typeparam>
public sealed class CursorPaginatedResponse<T>
{
    /// <summary>
    /// The data items.
    /// </summary>
    [JsonPropertyName("data")]
    public List<T> Data { get; init; } = [];

    /// <summary>
    /// Whether there are more results available after this page.
    /// </summary>
    [JsonPropertyName("has_more")]
    public bool HasMore { get; init; }

    /// <summary>
    /// Cursor to use as the "after" parameter for the next page.
    /// Null when there are no more results.
    /// </summary>
    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; init; }
}

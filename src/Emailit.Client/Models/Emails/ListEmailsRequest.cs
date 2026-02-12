namespace Emailit.Client.Models.Emails;

/// <summary>
/// Request model for listing emails with cursor-based pagination and filters.
/// </summary>
public sealed class ListEmailsRequest
{
    /// <summary>
    /// Maximum number of results to return (max 100, default 25).
    /// </summary>
    public int Limit { get; init; } = 25;

    /// <summary>
    /// Cursor for pagination. Use NextCursor from the previous response.
    /// </summary>
    public string? After { get; init; }

    /// <summary>
    /// Filter by email status (sent, delivered, opened, clicked, bounced, complained).
    /// </summary>
    public string? Status { get; init; }

    /// <summary>
    /// Filter by tag.
    /// </summary>
    public string? Tag { get; init; }

    /// <summary>
    /// Filter by sender email address.
    /// </summary>
    public string? From { get; init; }

    /// <summary>
    /// Filter by recipient email address.
    /// </summary>
    public string? To { get; init; }

    /// <summary>
    /// Filter by subject (partial match).
    /// </summary>
    public string? Subject { get; init; }

    /// <summary>
    /// Filter by creation date (ISO 8601). Returns emails created after this date.
    /// </summary>
    public string? CreatedAfter { get; init; }

    /// <summary>
    /// Filter by creation date (ISO 8601). Returns emails created before this date.
    /// </summary>
    public string? CreatedBefore { get; init; }
}

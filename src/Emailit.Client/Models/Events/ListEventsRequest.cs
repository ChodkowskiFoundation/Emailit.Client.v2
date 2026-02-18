namespace Emailit.Client.Models.Events;

/// <summary>
/// Request model for listing events with filters.
/// </summary>
public sealed class ListEventsRequest
{
    /// <summary>
    /// Page number (min: 1).
    /// </summary>
    public int Page { get; init; } = 1;

    /// <summary>
    /// Items per page (min: 1, max: 100).
    /// </summary>
    public int Limit { get; init; } = 100;

    /// <summary>
    /// Filter by event types (comma-separated, e.g., "email.accepted,email.delivered").
    /// </summary>
    public string? Type { get; init; }

    /// <summary>
    /// Include full event payload in response.
    /// </summary>
    public bool? IncludeData { get; init; }
}

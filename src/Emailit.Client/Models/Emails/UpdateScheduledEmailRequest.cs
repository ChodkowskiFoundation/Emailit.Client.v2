using System.Text.Json.Serialization;

namespace Emailit.Client.Models.Emails;

/// <summary>
/// Request to update a scheduled email's send time.
/// </summary>
public sealed class UpdateScheduledEmailRequest
{
    /// <summary>
    /// New scheduled send time (ISO 8601, Unix timestamp, or natural language).
    /// Must be at least 3 minutes in the future.
    /// </summary>
    [JsonPropertyName("scheduled_at")]
    public required string ScheduledAt { get; init; }
}

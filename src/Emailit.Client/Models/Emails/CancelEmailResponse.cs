using System.Text.Json.Serialization;

namespace Emailit.Client.Models.Emails;

/// <summary>
/// Response for email cancellation.
/// </summary>
public sealed record CancelEmailResponse
{
    /// <summary>
    /// Object type identifier.
    /// </summary>
    [JsonPropertyName("object")]
    public string Object { get; init; } = "email";

    /// <summary>
    /// Email ID that was cancelled.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = null!;

    /// <summary>
    /// Whether the cancellation was successful.
    /// </summary>
    [JsonPropertyName("cancelled")]
    public bool Cancelled { get; init; }
}

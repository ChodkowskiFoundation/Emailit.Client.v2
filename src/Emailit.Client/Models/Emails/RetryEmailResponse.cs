using System.Text.Json.Serialization;

namespace Emailit.Client.Models.Emails;

/// <summary>
/// Response model for email retry operations.
/// </summary>
public sealed record RetryEmailResponse
{
    /// <summary>
    /// Object type identifier.
    /// </summary>
    [JsonPropertyName("object")]
    public string Object { get; init; } = "email";

    /// <summary>
    /// New email ID for the retried email.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = null!;

    /// <summary>
    /// Original email ID that was retried.
    /// </summary>
    [JsonPropertyName("original_id")]
    public string? OriginalId { get; init; }

    /// <summary>
    /// Status of the retried email.
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; init; } = null!;

    /// <summary>
    /// Status message from API.
    /// </summary>
    [JsonPropertyName("message")]
    public string? Message { get; init; }
}

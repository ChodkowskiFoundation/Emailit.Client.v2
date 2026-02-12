using System.Text.Json.Serialization;

namespace Emailit.Client.Models;

/// <summary>
/// Error response from Emailit API.
/// </summary>
public sealed record ErrorResponse
{
    /// <summary>
    /// Error type.
    /// </summary>
    [JsonPropertyName("error")]
    public string? Error { get; init; }

    /// <summary>
    /// Error message.
    /// </summary>
    [JsonPropertyName("message")]
    public string? Message { get; init; }

    /// <summary>
    /// Error code.
    /// </summary>
    [JsonPropertyName("code")]
    public string? Code { get; init; }

    /// <summary>
    /// Error details.
    /// </summary>
    [JsonPropertyName("details")]
    public string? Details { get; init; }

    /// <summary>
    /// Validation errors by field.
    /// </summary>
    [JsonPropertyName("errors")]
    public Dictionary<string, string[]>? Errors { get; init; }

    /// <summary>
    /// Gets the best available error message.
    /// </summary>
    public string GetErrorMessage()
    {
        return Message ?? Error ?? Code ?? Details ?? "Unknown error";
    }
}

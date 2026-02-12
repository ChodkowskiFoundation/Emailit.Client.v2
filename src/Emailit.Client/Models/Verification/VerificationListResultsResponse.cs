using System.Text.Json.Serialization;

namespace Emailit.Client.Models.Verification;

/// <summary>
/// Response model for verification list results.
/// </summary>
public sealed record VerificationListResultsResponse
{
    /// <summary>
    /// Object type identifier.
    /// </summary>
    [JsonPropertyName("object")]
    public string Object { get; init; } = "email_verification_list_results";

    /// <summary>
    /// List identifier.
    /// </summary>
    [JsonPropertyName("list_id")]
    public string ListId { get; init; } = null!;

    /// <summary>
    /// Verification results.
    /// </summary>
    [JsonPropertyName("data")]
    public List<EmailVerificationResponse> Data { get; init; } = [];

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
}

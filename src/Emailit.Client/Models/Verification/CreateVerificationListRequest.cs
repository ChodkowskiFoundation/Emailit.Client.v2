using System.Text.Json.Serialization;

namespace Emailit.Client.Models.Verification;

/// <summary>
/// Request to create a bulk email verification list.
/// </summary>
public sealed class CreateVerificationListRequest
{
    /// <summary>
    /// Name for the verification list.
    /// </summary>
    [JsonPropertyName("name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Name { get; init; }

    /// <summary>
    /// List of email addresses to verify (max 100,000).
    /// </summary>
    [JsonPropertyName("emails")]
    public required IReadOnlyList<string> Emails { get; init; }
}

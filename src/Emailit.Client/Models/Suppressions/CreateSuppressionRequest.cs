using System.Text.Json.Serialization;

namespace Emailit.Client.Models.Suppressions;

/// <summary>
/// Request to create a suppression entry.
/// </summary>
public sealed class CreateSuppressionRequest
{
    /// <summary>
    /// Email address to suppress.
    /// </summary>
    [JsonPropertyName("email")]
    public required string Email { get; init; }

    /// <summary>
    /// Suppression type: "hard_bounce", "soft_bounce", "spam_complaint", "unsubscribe", "manual".
    /// </summary>
    [JsonPropertyName("type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Type { get; init; }

    /// <summary>
    /// Reason for suppression.
    /// </summary>
    [JsonPropertyName("reason")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Reason { get; init; }
}

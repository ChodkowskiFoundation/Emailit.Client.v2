using System.Text.Json.Serialization;

namespace Emailit.Client.Models.Suppressions;

/// <summary>
/// Request to update a suppression entry.
/// </summary>
public sealed class UpdateSuppressionRequest
{
    /// <summary>
    /// Email address to update.
    /// </summary>
    [JsonPropertyName("email")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Email { get; init; }

    /// <summary>
    /// Suppression type.
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

    /// <summary>
    /// Expiration datetime (ISO 8601). Null for permanent suppression.
    /// </summary>
    [JsonPropertyName("keep_until")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? KeepUntil { get; init; }
}

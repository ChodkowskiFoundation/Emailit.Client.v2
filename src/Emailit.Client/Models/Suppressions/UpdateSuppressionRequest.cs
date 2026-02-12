using System.Text.Json.Serialization;

namespace Emailit.Client.Models.Suppressions;

/// <summary>
/// Request to update a suppression entry.
/// </summary>
public sealed class UpdateSuppressionRequest
{
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
}

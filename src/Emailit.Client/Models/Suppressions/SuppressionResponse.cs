using System.Text.Json.Serialization;

namespace Emailit.Client.Models.Suppressions;

/// <summary>
/// Response model for suppression operations.
/// </summary>
public sealed record SuppressionResponse
{
    /// <summary>
    /// Object type identifier.
    /// </summary>
    [JsonPropertyName("object")]
    public string Object { get; init; } = "suppression";

    /// <summary>
    /// Unique suppression identifier (prefixed with sup_).
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = null!;

    /// <summary>
    /// Suppressed email address.
    /// </summary>
    [JsonPropertyName("email")]
    public string Email { get; init; } = null!;

    /// <summary>
    /// Suppression type.
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; init; }

    /// <summary>
    /// Reason for suppression.
    /// </summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; init; }

    /// <summary>
    /// Creation timestamp.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// Last update timestamp.
    /// </summary>
    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; init; }
}

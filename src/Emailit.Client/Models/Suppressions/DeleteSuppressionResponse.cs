using System.Text.Json.Serialization;

namespace Emailit.Client.Models.Suppressions;

/// <summary>
/// Response for suppression deletion.
/// </summary>
public sealed record DeleteSuppressionResponse
{
    /// <summary>
    /// Object type identifier.
    /// </summary>
    [JsonPropertyName("object")]
    public string Object { get; init; } = "suppression";

    /// <summary>
    /// Suppression ID that was deleted.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = null!;

    /// <summary>
    /// Whether the deletion was successful.
    /// </summary>
    [JsonPropertyName("deleted")]
    public bool Deleted { get; init; }
}

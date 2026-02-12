using System.Text.Json.Serialization;

namespace Emailit.Client.Models.Audiences;

/// <summary>
/// Response for audience deletion.
/// </summary>
public sealed record DeleteAudienceResponse
{
    /// <summary>
    /// Object type identifier.
    /// </summary>
    [JsonPropertyName("object")]
    public string Object { get; init; } = "audience";

    /// <summary>
    /// Audience ID that was deleted.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = null!;

    /// <summary>
    /// Whether the deletion was successful.
    /// </summary>
    [JsonPropertyName("deleted")]
    public bool Deleted { get; init; }
}

using System.Text.Json.Serialization;

namespace Emailit.Client.Models.Domains;

/// <summary>
/// Response for domain deletion.
/// </summary>
public sealed record DeleteDomainResponse
{
    /// <summary>
    /// Object type identifier.
    /// </summary>
    [JsonPropertyName("object")]
    public string Object { get; init; } = "domain";

    /// <summary>
    /// Domain ID that was deleted.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = null!;

    /// <summary>
    /// Whether the deletion was successful.
    /// </summary>
    [JsonPropertyName("deleted")]
    public bool Deleted { get; init; }
}

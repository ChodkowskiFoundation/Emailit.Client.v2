using System.Text.Json.Serialization;

namespace Emailit.Client.Models.Subscribers;

/// <summary>
/// Response for subscriber deletion.
/// </summary>
public sealed record DeleteSubscriberResponse
{
    /// <summary>
    /// Object type identifier.
    /// </summary>
    [JsonPropertyName("object")]
    public string Object { get; init; } = "subscriber";

    /// <summary>
    /// Subscriber ID that was deleted.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = null!;

    /// <summary>
    /// Whether the deletion was successful.
    /// </summary>
    [JsonPropertyName("deleted")]
    public bool Deleted { get; init; }
}

using System.Text.Json.Serialization;

namespace Emailit.Client.Models.Emails;

/// <summary>
/// Email tracking configuration.
/// </summary>
public sealed class EmailTrackingOptions
{
    /// <summary>
    /// Track email opens/loads.
    /// </summary>
    [JsonPropertyName("loads")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Loads { get; init; }

    /// <summary>
    /// Track link clicks.
    /// </summary>
    [JsonPropertyName("clicks")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Clicks { get; init; }
}

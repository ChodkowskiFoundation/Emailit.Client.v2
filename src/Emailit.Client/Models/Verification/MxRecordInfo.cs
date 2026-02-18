using System.Text.Json.Serialization;

namespace Emailit.Client.Models.Verification;

/// <summary>
/// MX record information for the verified domain.
/// </summary>
public sealed record MxRecordInfo
{
    /// <summary>
    /// MX record priority.
    /// </summary>
    [JsonPropertyName("priority")]
    public int Priority { get; init; }

    /// <summary>
    /// MX record exchange hostname.
    /// </summary>
    [JsonPropertyName("exchange")]
    public string Exchange { get; init; } = null!;
}

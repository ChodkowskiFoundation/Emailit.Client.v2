using System.Text.Json.Serialization;

namespace Emailit.Client.Models.Templates;

/// <summary>
/// Internal wrapper for template API responses that wrap data in {data: {...}, message: "..."}.
/// </summary>
internal sealed record TemplateDataResponse
{
    [JsonPropertyName("data")]
    public TemplateResponse? Data { get; init; }

    [JsonPropertyName("message")]
    public string? Message { get; init; }
}

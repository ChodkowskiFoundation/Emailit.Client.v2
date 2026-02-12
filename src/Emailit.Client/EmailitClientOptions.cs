namespace Emailit.Client;

/// <summary>
/// Configuration options for the Emailit API client.
/// </summary>
public sealed class EmailitClientOptions
{
    /// <summary>
    /// Configuration section name in appsettings.json
    /// </summary>
    public const string SectionName = "Emailit";

    /// <summary>
    /// Emailit API key. Required for authentication.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Base URL for the Emailit API. Defaults to production API.
    /// </summary>
    public string BaseUrl { get; set; } = "https://api.emailit.com";

    /// <summary>
    /// Request timeout in seconds. Defaults to 30 seconds.
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Validates the options and throws if invalid.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when required options are missing.</exception>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(ApiKey))
        {
            throw new InvalidOperationException(
                "Emailit API key is required. Configure it via appsettings.json, " +
                "Configure() action, or constructor parameter.");
        }

        if (string.IsNullOrWhiteSpace(BaseUrl))
        {
            throw new InvalidOperationException("Emailit BaseUrl cannot be empty.");
        }

        if (TimeoutSeconds <= 0)
        {
            throw new InvalidOperationException("TimeoutSeconds must be greater than 0.");
        }
    }
}

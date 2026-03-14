namespace Emailit.Client;

/// <summary>
/// Controls how much diagnostic context is captured in client exceptions.
/// </summary>
public enum EmailitExceptionDetailMode
{
    /// <summary>
    /// Capture only normalized error metadata such as status code, rate limits, and error code.
    /// </summary>
    Minimal = 0,

    /// <summary>
    /// Capture safe request metadata such as method, request URI without query string, and request identifiers.
    /// </summary>
    Safe = 1,

    /// <summary>
    /// Capture diagnostic response details such as a truncated response body and response headers.
    /// </summary>
    Diagnostic = 2
}

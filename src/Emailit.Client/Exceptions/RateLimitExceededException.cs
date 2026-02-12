using Emailit.Client.Models;

namespace Emailit.Client.Exceptions;

/// <summary>
/// Exception thrown when the per-second rate limit is exceeded (HTTP 429).
/// </summary>
public class RateLimitExceededException : EmailitException
{
    /// <summary>
    /// Rate limit information from the response headers.
    /// </summary>
    public RateLimitInfo? RateLimitInfo { get; }

    public RateLimitExceededException(RateLimitInfo? rateLimitInfo = null)
        : base("Rate limit exceeded. Too many requests per second.", 429)
    {
        RateLimitInfo = rateLimitInfo;
    }

    public RateLimitExceededException(string message, RateLimitInfo? rateLimitInfo = null)
        : base(message, 429)
    {
        RateLimitInfo = rateLimitInfo;
    }
}

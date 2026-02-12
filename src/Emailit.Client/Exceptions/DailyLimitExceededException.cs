using Emailit.Client.Models;

namespace Emailit.Client.Exceptions;

/// <summary>
/// Exception thrown when the daily sending limit is exceeded (HTTP 429).
/// </summary>
public sealed class DailyLimitExceededException : EmailitException
{
    /// <summary>
    /// Rate limit information from the response headers.
    /// </summary>
    public RateLimitInfo? RateLimitInfo { get; }

    /// <summary>
    /// Estimated time until the daily limit resets (midnight UTC).
    /// </summary>
    public TimeSpan TimeUntilReset
    {
        get
        {
            var now = DateTime.UtcNow;
            var midnight = now.Date.AddDays(1);
            return midnight - now;
        }
    }

    public DailyLimitExceededException(RateLimitInfo? rateLimitInfo = null)
        : base("Daily sending limit exceeded.", 429)
    {
        RateLimitInfo = rateLimitInfo;
    }

    public DailyLimitExceededException(string message, RateLimitInfo? rateLimitInfo = null)
        : base(message, 429)
    {
        RateLimitInfo = rateLimitInfo;
    }
}

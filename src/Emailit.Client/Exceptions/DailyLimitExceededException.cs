using Emailit.Client.Models;

namespace Emailit.Client.Exceptions;

/// <summary>
/// Exception thrown when the daily sending limit is exceeded (HTTP 429).
/// </summary>
public sealed class DailyLimitExceededException(RateLimitInfo? rateLimitInfo = null) 
    : RateLimitExceededException("Rate limit exceeded. Too many requests per day.", rateLimitInfo)
{
}

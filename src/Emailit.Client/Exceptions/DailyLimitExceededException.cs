using Emailit.Client.Models;

namespace Emailit.Client.Exceptions;

/// <summary>
/// Exception thrown when the daily sending limit is exceeded (HTTP 429).
/// </summary>
public sealed class DailyLimitExceededException : RateLimitExceededException
{
    public DailyLimitExceededException(RateLimitInfo? rateLimitInfo = null, EmailitExceptionContext? context = null, Exception? innerException = null)
        : base("Rate limit exceeded. Too many requests per day.", rateLimitInfo, context, innerException)
    {
    }

    public override string ProblemType => "urn:emailit:problem:daily-rate-limit";

    public override string ProblemTitle => "Emailit daily rate limit exceeded";
}

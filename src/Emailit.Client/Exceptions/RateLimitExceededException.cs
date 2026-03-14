using Emailit.Client.Models;

namespace Emailit.Client.Exceptions;

/// <summary>
/// Exception thrown when the per-second rate limit is exceeded (HTTP 429).
/// </summary>
public class RateLimitExceededException : EmailitApiException
{
    public RateLimitExceededException(RateLimitInfo? rateLimitInfo = null, EmailitExceptionContext? context = null, Exception? innerException = null)
        : this("Rate limit exceeded. Too many requests per second.", rateLimitInfo, context, innerException)
    {
    }

    public RateLimitExceededException(string message, RateLimitInfo? rateLimitInfo = null, EmailitExceptionContext? context = null, Exception? innerException = null)
        : base(message, (context ?? new EmailitExceptionContext()) with
        {
            StatusCode = 429,
            IsTransient = true,
            RateLimitInfo = rateLimitInfo ?? context?.RateLimitInfo
        }, innerException)
    {
    }

    public override string ProblemType => "urn:emailit:problem:rate-limit";

    public override string ProblemTitle => "Emailit rate limit exceeded";
}

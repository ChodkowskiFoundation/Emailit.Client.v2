using Emailit.Client.Models;

namespace Emailit.Client.Exceptions;

/// <summary>
/// Base exception for Emailit client failures.
/// </summary>
public class EmailitException : Exception
{
    public EmailitException(string message)
        : this(message, context: null, innerException: null)
    {
    }

    public EmailitException(string message, int statusCode)
        : this(message, new EmailitExceptionContext { StatusCode = statusCode }, innerException: null)
    {
    }

    public EmailitException(string message, int statusCode, string? errorCode)
        : this(message, new EmailitExceptionContext { StatusCode = statusCode, ErrorCode = errorCode }, innerException: null)
    {
    }

    public EmailitException(string message, Exception innerException)
        : this(message, context: null, innerException)
    {
    }

    public EmailitException(string message, EmailitExceptionContext? context, Exception? innerException = null)
        : base(message, innerException)
    {
        Context = context ?? new EmailitExceptionContext();
        StatusCode = Context.StatusCode;
        ErrorCode = Context.ErrorCode;
        ErrorType = Context.ErrorType;
        RequestMethod = Context.RequestMethod;
        RequestUri = Context.RequestUri;
        RequestPath = Context.RequestPath;
        RequestId = Context.RequestId;
        RateLimitInfo = Context.RateLimitInfo;
        IsTransient = Context.IsTransient;
        ResponseBody = Context.ResponseBody;
        ResponseHeaders = Context.ResponseHeaders;
    }

    public EmailitExceptionContext Context { get; }

    public int? StatusCode { get; }

    public string? ErrorCode { get; }

    public string? ErrorType { get; }

    public string? RequestMethod { get; }

    public Uri? RequestUri { get; }

    public string? RequestPath { get; }

    public string? RequestId { get; }

    public RateLimitInfo? RateLimitInfo { get; }

    public bool IsTransient { get; }

    public string? ResponseBody { get; }

    public IReadOnlyDictionary<string, string>? ResponseHeaders { get; }

    public virtual string ProblemType => "urn:emailit:problem:client";

    public virtual string ProblemTitle => "Emailit client error";

    public EmailitProblemDetails ToProblemDetails(bool includeExceptionDetails = false)
    {
        var extensions = new Dictionary<string, object?>();

        if (!string.IsNullOrWhiteSpace(ErrorCode))
            extensions["errorCode"] = ErrorCode;

        if (!string.IsNullOrWhiteSpace(ErrorType))
            extensions["errorType"] = ErrorType;

        if (!string.IsNullOrWhiteSpace(RequestId))
            extensions["requestId"] = RequestId;

        if (!string.IsNullOrWhiteSpace(RequestMethod))
            extensions["requestMethod"] = RequestMethod;

        if (RequestUri is not null)
            extensions["requestUri"] = RequestUri.ToString();

        if (RateLimitInfo is not null)
        {
            extensions["rateLimit"] = new Dictionary<string, object?>
            {
                ["limit"] = RateLimitInfo.Limit,
                ["remaining"] = RateLimitInfo.Remaining,
                ["dailyLimit"] = RateLimitInfo.DailyLimit,
                ["dailyRemaining"] = RateLimitInfo.DailyRemaining,
                ["retryAfterSeconds"] = RateLimitInfo.RetryAfterSeconds
            };
        }

        extensions["isTransient"] = IsTransient;

        if (!string.IsNullOrWhiteSpace(ResponseBody))
            extensions["responseBody"] = ResponseBody;

        if (ResponseHeaders is { Count: > 0 })
            extensions["responseHeaders"] = ResponseHeaders;

        AddProblemDetailsExtensions(extensions);

        if (includeExceptionDetails)
        {
            extensions["exceptionType"] = GetType().FullName;

            if (!string.IsNullOrWhiteSpace(StackTrace))
                extensions["stackTrace"] = StackTrace;
        }

        return new EmailitProblemDetails
        {
            Type = ProblemType,
            Title = ProblemTitle,
            Status = StatusCode,
            Detail = Message,
            Instance = RequestPath,
            Extensions = extensions
        };
    }

    protected virtual void AddProblemDetailsExtensions(IDictionary<string, object?> extensions)
    {
    }
}

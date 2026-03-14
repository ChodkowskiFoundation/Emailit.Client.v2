using Emailit.Client.Models;

namespace Emailit.Client.Exceptions;

/// <summary>
/// Diagnostic context captured for an Emailit client failure.
/// </summary>
public sealed record EmailitExceptionContext
{
    public int? StatusCode { get; init; }

    public string? ErrorCode { get; init; }

    public string? ErrorType { get; init; }

    public string? RequestMethod { get; init; }

    public Uri? RequestUri { get; init; }

    public string? RequestPath { get; init; }

    public string? RequestId { get; init; }

    public RateLimitInfo? RateLimitInfo { get; init; }

    public bool IsTransient { get; init; }

    public string? ResponseBody { get; init; }

    public IReadOnlyDictionary<string, string>? ResponseHeaders { get; init; }
}

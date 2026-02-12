namespace Emailit.Client.Models;

/// <summary>
/// Rate limit information from Emailit API response headers.
/// </summary>
public sealed class RateLimitInfo
{
    /// <summary>
    /// Maximum requests per second.
    /// </summary>
    public int Limit { get; init; }

    /// <summary>
    /// Remaining requests in current second window.
    /// </summary>
    public int Remaining { get; init; }

    /// <summary>
    /// Maximum requests per day.
    /// </summary>
    public int DailyLimit { get; init; }

    /// <summary>
    /// Remaining requests for today.
    /// </summary>
    public int DailyRemaining { get; init; }

    /// <summary>
    /// Seconds to wait before retrying (from Retry-After header).
    /// </summary>
    public int? RetryAfterSeconds { get; init; }

    /// <summary>
    /// Whether the daily limit has been reached.
    /// </summary>
    public bool IsDailyLimitReached => DailyRemaining <= 0 && DailyLimit > 0;

    /// <summary>
    /// Whether the per-second rate limit has been reached.
    /// </summary>
    public bool IsRateLimitReached => Remaining <= 0 && Limit > 0;

    /// <summary>
    /// Parses rate limit info from HTTP response headers.
    /// </summary>
    /// <param name="headers">Dictionary of header names to values (case-insensitive).</param>
    /// <returns>Parsed rate limit information.</returns>
    public static RateLimitInfo FromHeaders(IReadOnlyDictionary<string, string> headers)
    {
        return new RateLimitInfo
        {
            Limit = GetIntHeader(headers, "ratelimit-limit"),
            Remaining = GetIntHeader(headers, "ratelimit-remaining"),
            DailyLimit = GetIntHeader(headers, "ratelimit-daily-limit"),
            DailyRemaining = GetIntHeader(headers, "ratelimit-daily-remaining"),
            RetryAfterSeconds = GetNullableIntHeader(headers, "retry-after")
        };
    }

    private static int GetIntHeader(IReadOnlyDictionary<string, string> headers, string name)
    {
        var value = FindHeaderValue(headers, name);
        if (value != null && int.TryParse(value, out var result))
        {
            return result;
        }
        return 0;
    }

    private static int? GetNullableIntHeader(IReadOnlyDictionary<string, string> headers, string name)
    {
        var value = FindHeaderValue(headers, name);
        if (value != null && int.TryParse(value, out var result))
        {
            return result;
        }
        return null;
    }

    private static string? FindHeaderValue(IReadOnlyDictionary<string, string> headers, string name)
    {
        // First try exact match (most common case)
        if (headers.TryGetValue(name, out var value))
        {
            return value;
        }

        // Fall back to case-insensitive search
        foreach (var kvp in headers)
        {
            if (string.Equals(kvp.Key, name, StringComparison.OrdinalIgnoreCase))
            {
                return kvp.Value;
            }
        }

        return null;
    }
}

namespace Emailit.Client.Exceptions;

/// <summary>
/// RFC 7807-style problem details payload for Emailit client failures.
/// </summary>
public sealed record EmailitProblemDetails
{
    public string Type { get; init; } = "about:blank";

    public string Title { get; init; } = "Emailit client error";

    public int? Status { get; init; }

    public string? Detail { get; init; }

    public string? Instance { get; init; }

    public IReadOnlyDictionary<string, object?> Extensions { get; init; } = new Dictionary<string, object?>();
}

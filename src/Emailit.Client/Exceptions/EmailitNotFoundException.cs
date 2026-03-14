namespace Emailit.Client.Exceptions;

/// <summary>
/// Exception thrown when a resource is not found (HTTP 404).
/// </summary>
public sealed class EmailitNotFoundException : EmailitApiException
{
    /// <summary>
    /// The resource type that was not found.
    /// </summary>
    public string? ResourceType { get; }

    /// <summary>
    /// The ID of the resource that was not found.
    /// </summary>
    public string? ResourceId { get; }

    public EmailitNotFoundException(string message, EmailitExceptionContext? context = null, Exception? innerException = null)
        : base(message, (context ?? new EmailitExceptionContext()) with { StatusCode = 404 }, innerException)
    {
    }

    public EmailitNotFoundException(string resourceType, string resourceId, EmailitExceptionContext? context = null, Exception? innerException = null)
        : base($"{resourceType} with ID '{resourceId}' was not found.", (context ?? new EmailitExceptionContext()) with { StatusCode = 404 }, innerException)
    {
        ResourceType = resourceType;
        ResourceId = resourceId;
    }

    public override string ProblemType => "urn:emailit:problem:not-found";

    public override string ProblemTitle => "Emailit resource was not found";
}

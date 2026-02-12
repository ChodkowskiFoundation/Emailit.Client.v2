namespace Emailit.Client.Exceptions;

/// <summary>
/// Exception thrown when a resource is not found (HTTP 404).
/// </summary>
public sealed class EmailitNotFoundException : EmailitException
{
    /// <summary>
    /// The resource type that was not found.
    /// </summary>
    public string? ResourceType { get; }

    /// <summary>
    /// The ID of the resource that was not found.
    /// </summary>
    public string? ResourceId { get; }

    public EmailitNotFoundException(string message)
        : base(message, 404)
    {
    }

    public EmailitNotFoundException(string resourceType, string resourceId)
        : base($"{resourceType} with ID '{resourceId}' was not found.", 404)
    {
        ResourceType = resourceType;
        ResourceId = resourceId;
    }
}

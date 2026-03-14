namespace Emailit.Client.Exceptions;

/// <summary>
/// Exception thrown when the Emailit API accepts the request shape but rejects the operation because the resource state is not processable (HTTP 422).
/// </summary>
public sealed class EmailitUnprocessableEntityException : EmailitApiException
{
    public EmailitUnprocessableEntityException(
        string message,
        IReadOnlyDictionary<string, string[]>? errors = null,
        EmailitExceptionContext? context = null,
        Exception? innerException = null)
        : base(message, (context ?? new EmailitExceptionContext()) with { StatusCode = 422 }, innerException)
    {
        ValidationErrors = errors;
    }

    /// <summary>
    /// Optional field-level errors returned by the API.
    /// </summary>
    public IReadOnlyDictionary<string, string[]>? ValidationErrors { get; }

    public override string ProblemType => "urn:emailit:problem:unprocessable-entity";

    public override string ProblemTitle => "Emailit request could not be processed";

    protected override void AddProblemDetailsExtensions(IDictionary<string, object?> extensions)
    {
        if (ValidationErrors is { Count: > 0 })
            extensions["errors"] = ValidationErrors;
    }
}

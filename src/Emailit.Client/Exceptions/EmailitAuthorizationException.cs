namespace Emailit.Client.Exceptions;

/// <summary>
/// Exception thrown when the API key is valid but lacks permission to perform the requested action.
/// </summary>
public sealed class EmailitAuthorizationException : EmailitApiException
{
    public EmailitAuthorizationException(string message, EmailitExceptionContext? context = null, Exception? innerException = null)
        : base(message, (context ?? new EmailitExceptionContext()) with { StatusCode = 403 }, innerException)
    {
    }

    public override string ProblemType => "urn:emailit:problem:authorization";

    public override string ProblemTitle => "Emailit request is not authorized";
}

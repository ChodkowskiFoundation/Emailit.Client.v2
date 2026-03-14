namespace Emailit.Client.Exceptions;

/// <summary>
/// Exception thrown when the Emailit API rejects a request because the current resource state conflicts with the operation.
/// </summary>
public sealed class EmailitConflictException : EmailitApiException
{
    public EmailitConflictException(string message, EmailitExceptionContext? context = null, Exception? innerException = null)
        : base(message, (context ?? new EmailitExceptionContext()) with { StatusCode = 409 }, innerException)
    {
    }

    public override string ProblemType => "urn:emailit:problem:conflict";

    public override string ProblemTitle => "Emailit resource conflict";
}

namespace Emailit.Client.Exceptions;

/// <summary>
/// Exception thrown when authentication fails (HTTP 401).
/// </summary>
public sealed class EmailitAuthenticationException : EmailitApiException
{
    public EmailitAuthenticationException()
        : this("Invalid or missing API key.")
    {
    }

    public EmailitAuthenticationException(string message, EmailitExceptionContext? context = null, Exception? innerException = null)
        : base(message, (context ?? new EmailitExceptionContext()) with { StatusCode = 401 }, innerException)
    {
    }

    public override string ProblemType => "urn:emailit:problem:authentication";

    public override string ProblemTitle => "Emailit authentication failed";
}

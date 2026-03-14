namespace Emailit.Client.Exceptions;

/// <summary>
/// Base exception for HTTP responses returned by the Emailit API.
/// </summary>
public class EmailitApiException : EmailitException
{
    public EmailitApiException(string message, EmailitExceptionContext? context = null, Exception? innerException = null)
        : base(message, context, innerException)
    {
    }

    public override string ProblemType => "urn:emailit:problem:api";

    public override string ProblemTitle => "Emailit API request failed";
}

namespace Emailit.Client.Exceptions;

/// <summary>
/// Exception thrown when the Emailit API returns an unexpected server-side failure.
/// </summary>
public sealed class EmailitServerException : EmailitApiException
{
    public EmailitServerException(string message, EmailitExceptionContext? context = null, Exception? innerException = null)
        : base(message, context, innerException)
    {
    }

    public override string ProblemType => "urn:emailit:problem:server";

    public override string ProblemTitle => "Emailit server error";
}

namespace Emailit.Client.Exceptions;

/// <summary>
/// Exception thrown when a request to the Emailit API exceeds the configured timeout.
/// </summary>
public sealed class EmailitTimeoutException : EmailitTransportException
{
    public EmailitTimeoutException(string message, EmailitExceptionContext? context = null, Exception? innerException = null)
        : base(message, (context ?? new EmailitExceptionContext()) with { IsTransient = true }, innerException)
    {
    }

    public override string ProblemType => "urn:emailit:problem:timeout";

    public override string ProblemTitle => "Emailit request timed out";
}

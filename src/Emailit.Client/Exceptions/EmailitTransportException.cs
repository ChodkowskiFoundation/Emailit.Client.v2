namespace Emailit.Client.Exceptions;

/// <summary>
/// Exception thrown when the client cannot reach the Emailit API or no HTTP response is received.
/// </summary>
public class EmailitTransportException : EmailitException
{
    public EmailitTransportException(string message, EmailitExceptionContext? context = null, Exception? innerException = null)
        : base(message, (context ?? new EmailitExceptionContext()) with { IsTransient = true }, innerException)
    {
    }

    public override string ProblemType => "urn:emailit:problem:transport";

    public override string ProblemTitle => "Emailit transport failure";
}

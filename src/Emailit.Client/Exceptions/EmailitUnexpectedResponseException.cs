namespace Emailit.Client.Exceptions;

/// <summary>
/// Exception thrown when the Emailit API responds successfully but the payload shape does not match the expected contract.
/// </summary>
public sealed class EmailitUnexpectedResponseException : EmailitException
{
    public EmailitUnexpectedResponseException(string message, EmailitExceptionContext? context = null, Exception? innerException = null)
        : base(message, context, innerException)
    {
    }

    public override string ProblemType => "urn:emailit:problem:unexpected-response";

    public override string ProblemTitle => "Emailit returned an unexpected response";
}

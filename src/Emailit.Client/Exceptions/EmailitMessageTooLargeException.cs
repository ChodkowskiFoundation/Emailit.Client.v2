namespace Emailit.Client.Exceptions;

/// <summary>
/// Exception thrown when the Emailit API rejects a message because it exceeds the maximum supported size.
/// </summary>
public sealed class EmailitMessageTooLargeException : EmailitApiException
{
    public static int MaxSizeInMB => 40;

    public EmailitMessageTooLargeException()
        : this($"Message size exceeds maximum allowed size of {MaxSizeInMB}MB")
    {
    }

    public EmailitMessageTooLargeException(string message, EmailitExceptionContext? context = null, Exception? innerException = null)
        : base(message, (context ?? new EmailitExceptionContext()) with { StatusCode = 413 }, innerException)
    {
    }

    public override string ProblemType => "urn:emailit:problem:message-too-large";

    public override string ProblemTitle => "Emailit message is too large";
}

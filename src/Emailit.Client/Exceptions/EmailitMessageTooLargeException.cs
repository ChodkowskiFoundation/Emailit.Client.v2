namespace Emailit.Client.Exceptions;

internal class EmailitMessageTooLargeException : EmailitException
{
    public static int MaxSizeInMB => 40;

    public EmailitMessageTooLargeException()
    : base($"Message size exceeds maximum allowed size of {MaxSizeInMB}MB", 413)
    {
    }

    public EmailitMessageTooLargeException(string message)
        : base(message, 413)
    {
    }
}

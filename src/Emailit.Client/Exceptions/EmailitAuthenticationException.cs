namespace Emailit.Client.Exceptions;

/// <summary>
/// Exception thrown when authentication fails (HTTP 401).
/// </summary>
public sealed class EmailitAuthenticationException : EmailitException
{
    public EmailitAuthenticationException()
        : base("Invalid or missing API key.", 401)
    {
    }

    public EmailitAuthenticationException(string message)
        : base(message, 401)
    {
    }
}

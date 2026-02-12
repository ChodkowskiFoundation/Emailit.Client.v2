namespace Emailit.Client.Exceptions;

/// <summary>
/// Base exception for Emailit API errors.
/// </summary>
public class EmailitException : Exception
{
    /// <summary>
    /// HTTP status code from the API response.
    /// </summary>
    public int? StatusCode { get; }

    /// <summary>
    /// Error code from the API response.
    /// </summary>
    public string? ErrorCode { get; }

    public EmailitException(string message) : base(message)
    {
    }

    public EmailitException(string message, int statusCode) : base(message)
    {
        StatusCode = statusCode;
    }

    public EmailitException(string message, int statusCode, string? errorCode) : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }

    public EmailitException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

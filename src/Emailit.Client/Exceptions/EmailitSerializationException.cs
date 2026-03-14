namespace Emailit.Client.Exceptions;

/// <summary>
/// Exception thrown when the client cannot deserialize a response payload returned by Emailit.
/// </summary>
public sealed class EmailitSerializationException : EmailitException
{
    public EmailitSerializationException(string message, EmailitExceptionContext? context = null, Exception? innerException = null)
        : base(message, context, innerException)
    {
    }

    public override string ProblemType => "urn:emailit:problem:serialization";

    public override string ProblemTitle => "Emailit response deserialization failed";
}

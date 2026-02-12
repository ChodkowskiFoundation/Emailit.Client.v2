namespace Emailit.Client.Exceptions;

/// <summary>
/// Exception thrown when the API returns a validation error (HTTP 400).
/// </summary>
public sealed class EmailitValidationException(string message, IReadOnlyDictionary<string, string[]>? errors = null) 
    : EmailitException(message, 400)
{
    /// <summary>
    /// Validation errors by field name.
    /// </summary>
    public IReadOnlyDictionary<string, string[]>? ValidationErrors { get; } = errors;
}

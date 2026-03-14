using System.Security.Cryptography;
using System.Text;

namespace Emailit.Client.Webhooks;

/// <summary>
/// Validates HMAC-SHA256 signatures on incoming Emailit webhook requests.
/// </summary>
public static class WebhookSignatureValidator
{
    /// <summary>
    /// Validates a webhook signature using timing-safe comparison.
    /// </summary>
    /// <param name="payload">Raw request body.</param>
    /// <param name="signature">Value of the X-Emailit-Signature header.</param>
    /// <param name="timestamp">Value of the X-Emailit-Timestamp header.</param>
    /// <param name="secret">Webhook signing secret.</param>
    /// <returns>True if the signature is valid.</returns>
    public static bool ValidateSignature(string payload, string signature, string timestamp, string secret)
    {
        var expected = ComputeSignature(payload, timestamp, secret);

        var expectedBytes = Encoding.UTF8.GetBytes(expected);
        var actualBytes = Encoding.UTF8.GetBytes(signature);

        return CryptographicOperations.FixedTimeEquals(expectedBytes, actualBytes);
    }

    /// <summary>
    /// Validates a webhook signature with replay attack protection.
    /// </summary>
    /// <param name="payload">Raw request body.</param>
    /// <param name="signature">Value of the X-Emailit-Signature header.</param>
    /// <param name="timestamp">Value of the X-Emailit-Timestamp header.</param>
    /// <param name="secret">Webhook signing secret.</param>
    /// <param name="clockTolerance">Maximum age of the timestamp before rejection.</param>
    /// <returns>True if the signature is valid and the timestamp is within tolerance.</returns>
    /// <exception cref="ArgumentException">Thrown when the timestamp is not a valid Unix timestamp.</exception>
    public static bool ValidateSignature(
        string payload,
        string signature,
        string timestamp,
        string secret,
        TimeSpan clockTolerance)
    {
        if (!long.TryParse(timestamp, out var unixSeconds))
            throw new ArgumentException("Timestamp is not a valid Unix timestamp.", nameof(timestamp));

        var webhookTime = DateTimeOffset.FromUnixTimeSeconds(unixSeconds);
        var age = DateTimeOffset.UtcNow - webhookTime;

        if (TimeSpan.Abs(age) > clockTolerance)
            return false;

        return ValidateSignature(payload, signature, timestamp, secret);
    }

    /// <summary>
    /// Computes the expected HMAC-SHA256 signature for a webhook payload.
    /// </summary>
    /// <param name="payload">Raw request body.</param>
    /// <param name="timestamp">Unix timestamp string.</param>
    /// <param name="secret">Webhook signing secret.</param>
    /// <returns>Hex-encoded HMAC-SHA256 digest.</returns>
    public static string ComputeSignature(string payload, string timestamp, string secret)
    {
        var signedContent = $"{timestamp}.{payload}";
        var keyBytes = Encoding.UTF8.GetBytes(secret);
        var contentBytes = Encoding.UTF8.GetBytes(signedContent);

        var hash = HMACSHA256.HashData(keyBytes, contentBytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}

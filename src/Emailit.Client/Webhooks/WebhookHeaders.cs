namespace Emailit.Client.Webhooks;

/// <summary>
/// HTTP header names used by Emailit webhook deliveries.
/// </summary>
public static class WebhookHeaders
{
    /// <summary>
    /// HMAC-SHA256 hex digest of the webhook payload.
    /// </summary>
    public const string Signature = "X-Emailit-Signature";

    /// <summary>
    /// Unix timestamp used in signature computation.
    /// </summary>
    public const string Timestamp = "X-Emailit-Timestamp";
}

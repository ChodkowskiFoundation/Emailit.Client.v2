namespace Emailit.Client.Models.Emails;

/// <summary>
/// Email status constants as defined by Emailit API v2.
/// Use these values with <see cref="ListEmailsRequest.Status"/> or to compare against <see cref="EmailResponse.Status"/>.
/// </summary>
public static class EmailStatus
{
    /// <summary>Email accepted for delivery.</summary>
    public const string Accepted = "accepted";

    /// <summary>Email scheduled for future delivery.</summary>
    public const string Scheduled = "scheduled";

    /// <summary>Email successfully delivered.</summary>
    public const string Delivered = "delivered";

    /// <summary>Email bounced (hard bounce).</summary>
    public const string Bounced = "bounced";

    /// <summary>Delivery attempted but soft-bounced; may be retried.</summary>
    public const string Attempted = "attempted";

    /// <summary>Email delivery failed.</summary>
    public const string Failed = "failed";

    /// <summary>Email rejected by the system (e.g., suppressed sender).</summary>
    public const string Rejected = "rejected";

    /// <summary>Email was loaded/opened by the recipient.</summary>
    public const string Loaded = "loaded";

    /// <summary>A link in the email was clicked.</summary>
    public const string Clicked = "clicked";

    /// <summary>Email skipped due to suppression rules.</summary>
    public const string Suppressed = "suppressed";

    /// <summary>Inbound email received.</summary>
    public const string Received = "received";

    /// <summary>Recipient filed a spam complaint.</summary>
    public const string Complained = "complained";
}

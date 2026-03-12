namespace Emailit.Client.Models.Emails;

/// <summary>
/// Email type constants (inbound/outbound).
/// Use with <see cref="ListEmailsRequest.Type"/> or to compare against <see cref="EmailResponse.Type"/>.
/// </summary>
public static class EmailType
{
    /// <summary>Inbound (received) email.</summary>
    public const string Inbound = "inbound";

    /// <summary>Outbound (sent) email.</summary>
    public const string Outbound = "outbound";
}

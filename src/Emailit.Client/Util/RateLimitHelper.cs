namespace Emailit.Client.Util;

public static class RateLimitHelper
{
    /// <summary>
    /// Gets estimated time until the daily limit resets (midnight UTC).
    /// </summary>
    public static TimeSpan GetTimeUntilReset()
    {
        var now = DateTime.UtcNow;
        var midnight = now.Date.AddDays(1);
        return midnight - now;
    }
}

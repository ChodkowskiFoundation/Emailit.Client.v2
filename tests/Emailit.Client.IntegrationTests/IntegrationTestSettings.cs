namespace Emailit.Client.IntegrationTests;

internal sealed class IntegrationTestSettings
{
    public required string ApiKey { get; init; }
    public required string SendingDomain { get; init; }
    public required string RecipientEmail { get; init; }
    public required string BaseUrl { get; init; }
    public required int TimeoutSeconds { get; init; }

    public static IntegrationTestSettings Load() => new()
    {
        ApiKey = GetRequired("EMAILIT_INTEGRATION_API_KEY"),
        SendingDomain = GetOptional("EMAILIT_INTEGRATION_DOMAIN", "chodkowski.org"),
        RecipientEmail = GetOptional("EMAILIT_INTEGRATION_TO_EMAIL", "chodkowskikonstanty@gmail.com"),
        BaseUrl = GetOptional("EMAILIT_INTEGRATION_BASE_URL", "https://api.emailit.com"),
        TimeoutSeconds = GetOptionalInt("EMAILIT_INTEGRATION_TIMEOUT_SECONDS", 60)
    };

    private static string GetRequired(string name)
    {
        var value = Environment.GetEnvironmentVariable(name);
        if (!string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        throw new InvalidOperationException(
            $"Missing required environment variable '{name}'. " +
            "Set EMAILIT_INTEGRATION_API_KEY before running production integration tests.");
    }

    private static string GetOptional(string name, string fallback)
    {
        var value = Environment.GetEnvironmentVariable(name);
        return string.IsNullOrWhiteSpace(value) ? fallback : value;
    }

    private static int GetOptionalInt(string name, int fallback)
    {
        var value = Environment.GetEnvironmentVariable(name);
        return int.TryParse(value, out var parsed) && parsed > 0 ? parsed : fallback;
    }
}

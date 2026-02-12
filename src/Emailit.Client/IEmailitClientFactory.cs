namespace Emailit.Client;

/// <summary>
/// Factory for creating Emailit clients with different API keys.
/// Useful for multi-tenant scenarios where each tenant has their own API key.
/// </summary>
public interface IEmailitClientFactory : IDisposable
{
    /// <summary>
    /// Creates a new client instance configured with the specified API key.
    /// </summary>
    /// <param name="apiKey">The API key to use for this client.</param>
    /// <returns>A configured Emailit client.</returns>
    IEmailitClient CreateClient(string apiKey);
}

using System.Collections.Concurrent;
using Microsoft.Extensions.Options;

namespace Emailit.Client;

/// <summary>
/// Factory for creating Emailit clients with different API keys.
/// Used in multi-tenant scenarios where each tenant has their own API key.
/// Clients are cached per API key and reused to avoid socket exhaustion.
/// </summary>
public sealed class EmailitClientFactory : IEmailitClientFactory
{
    private readonly EmailitClientOptions _baseOptions;
    private readonly ConcurrentDictionary<string, IEmailitClient> _clients = new();

    /// <summary>
    /// Creates a factory with the specified base options.
    /// The API key in the options is ignored — it is provided per-client via CreateClient.
    /// </summary>
    public EmailitClientFactory(EmailitClientOptions options)
    {
        _baseOptions = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// Creates a factory with base options from IOptions (for DI).
    /// </summary>
    public EmailitClientFactory(IOptions<EmailitClientOptions> options)
        : this(options.Value)
    {
    }

    /// <inheritdoc />
    public IEmailitClient CreateClient(string apiKey)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new ArgumentException("API key is required.", nameof(apiKey));
        }

        return _clients.GetOrAdd(apiKey, key => new EmailitClient(new EmailitClientOptions
        {
            ApiKey = key,
            BaseUrl = _baseOptions.BaseUrl,
            TimeoutSeconds = _baseOptions.TimeoutSeconds
        }));
    }

    public void Dispose()
    {
        foreach (var client in _clients.Values)
        {
            client.Dispose();
        }

        _clients.Clear();
    }
}

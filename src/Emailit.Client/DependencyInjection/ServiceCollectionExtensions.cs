using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Emailit.Client.DependencyInjection;

/// <summary>
/// Extension methods for registering Emailit client in DI container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers Emailit client using configuration from appsettings.json.
    /// Configuration section: "Emailit"
    /// </summary>
    /// <example>
    /// appsettings.json:
    /// {
    ///   "Emailit": {
    ///     "ApiKey": "em_xxxxx",
    ///     "BaseUrl": "https://api.emailit.com",
    ///     "TimeoutSeconds": 30
    ///   }
    /// }
    /// </example>
    public static IServiceCollection AddEmailitClient(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<EmailitClientOptions>(
            configuration.GetSection(EmailitClientOptions.SectionName));

        services.AddSingleton<IEmailitClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<EmailitClientOptions>>();
            return new EmailitClient(options);
        });

        return services;
    }

    /// <summary>
    /// Registers Emailit client using a configuration action.
    /// </summary>
    /// <example>
    /// services.AddEmailitClient(options =>
    /// {
    ///     options.ApiKey = "em_xxxxx";
    ///     options.TimeoutSeconds = 60;
    /// });
    /// </example>
    public static IServiceCollection AddEmailitClient(
        this IServiceCollection services,
        Action<EmailitClientOptions> configure)
    {
        services.Configure(configure);

        services.AddSingleton<IEmailitClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<EmailitClientOptions>>();
            return new EmailitClient(options);
        });

        return services;
    }

    /// <summary>
    /// Registers Emailit client with just an API key (uses default settings).
    /// </summary>
    /// <example>
    /// services.AddEmailitClient("em_xxxxx");
    /// </example>
    public static IServiceCollection AddEmailitClient(
        this IServiceCollection services,
        string apiKey)
    {
        return services.AddEmailitClient(options => options.ApiKey = apiKey);
    }

    /// <summary>
    /// Registers Emailit client using both appsettings and a configuration action.
    /// The action is applied after appsettings, allowing overrides.
    /// </summary>
    /// <example>
    /// services.AddEmailitClient(configuration, options =>
    /// {
    ///     options.TimeoutSeconds = 60; // Override specific settings
    /// });
    /// </example>
    public static IServiceCollection AddEmailitClient(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<EmailitClientOptions> configure)
    {
        services.Configure<EmailitClientOptions>(
            configuration.GetSection(EmailitClientOptions.SectionName));

        services.PostConfigure(configure);

        services.AddSingleton<IEmailitClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<EmailitClientOptions>>();
            return new EmailitClient(options);
        });

        return services;
    }

    /// <summary>
    /// Registers Emailit client factory for multi-tenant scenarios.
    /// Use this when each tenant has their own API key.
    /// </summary>
    /// <example>
    /// services.AddEmailitClientFactory(options =>
    /// {
    ///     options.BaseUrl = "https://api.emailit.com";
    ///     options.TimeoutSeconds = 30;
    /// });
    ///
    /// // Then in your service:
    /// var client = _factory.CreateClient(tenantApiKey);
    /// </example>
    public static IServiceCollection AddEmailitClientFactory(
        this IServiceCollection services,
        Action<EmailitClientOptions>? configure = null)
    {
        if (configure != null)
        {
            services.Configure(configure);
        }

        services.AddSingleton<IEmailitClientFactory>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<EmailitClientOptions>>();
            return new EmailitClientFactory(options);
        });

        return services;
    }

    /// <summary>
    /// Registers Emailit client factory using configuration from appsettings.json.
    /// BaseUrl and TimeoutSeconds are read from config; API key is set per-client.
    /// </summary>
    public static IServiceCollection AddEmailitClientFactory(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<EmailitClientOptions>(
            configuration.GetSection(EmailitClientOptions.SectionName));

        services.AddSingleton<IEmailitClientFactory>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<EmailitClientOptions>>();
            return new EmailitClientFactory(options);
        });

        return services;
    }

}

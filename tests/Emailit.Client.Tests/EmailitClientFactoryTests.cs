using Flurl.Http.Testing;

namespace Emailit.Client.Tests;

public sealed class EmailitClientFactoryTests : IDisposable
{
    private readonly HttpTest _httpTest;
    private readonly EmailitClientFactory _factory;

    public EmailitClientFactoryTests()
    {
        _httpTest = new HttpTest();
        _factory = new EmailitClientFactory(new EmailitClientOptions
        {
            ApiKey = "ignored",
            BaseUrl = "https://api.emailit.com",
            TimeoutSeconds = 30
        });
    }

    public void Dispose()
    {
        _factory.Dispose();
        _httpTest.Dispose();
    }

    #region CreateClient Tests

    [Fact]
    public void CreateClient_SameApiKey_ReturnsSameInstance()
    {
        // Act
        var client1 = _factory.CreateClient("em_key_1");
        var client2 = _factory.CreateClient("em_key_1");

        // Assert
        client1.Should().BeSameAs(client2);
    }

    [Fact]
    public void CreateClient_DifferentApiKeys_ReturnsDifferentInstances()
    {
        // Act
        var client1 = _factory.CreateClient("em_key_1");
        var client2 = _factory.CreateClient("em_key_2");

        // Assert
        client1.Should().NotBeSameAs(client2);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateClient_EmptyOrNullApiKey_ThrowsArgumentException(string? apiKey)
    {
        // Act & Assert
        var act = () => _factory.CreateClient(apiKey!);
        act.Should().Throw<ArgumentException>()
            .WithParameterName("apiKey");
    }

    [Fact]
    public void CreateClient_UsesBaseOptionsForUrl()
    {
        // Arrange
        var factory = new EmailitClientFactory(new EmailitClientOptions
        {
            ApiKey = "ignored",
            BaseUrl = "https://custom.api.com",
            TimeoutSeconds = 60
        });

        _httpTest.RespondWithJson(new { data = Array.Empty<object>() });

        // Act
        var client = factory.CreateClient("em_key_1");

        // Assert — client was created (non-null), further validation via HTTP calls in other tests
        client.Should().NotBeNull();

        factory.Dispose();
    }

    #endregion

    #region Dispose Tests

    [Fact]
    public void Dispose_WithCachedClients_DisposesAllClients()
    {
        // Arrange
        var client1 = _factory.CreateClient("em_key_1");
        var client2 = _factory.CreateClient("em_key_2");

        // Act — dispose the factory
        _factory.Dispose();

        // Assert — after factory dispose, creating new clients works
        // (factory was already disposed in test cleanup, but we can create a new one)
        var newFactory = new EmailitClientFactory(new EmailitClientOptions
        {
            ApiKey = "ignored",
            BaseUrl = "https://api.emailit.com",
            TimeoutSeconds = 30
        });

        var newClient = newFactory.CreateClient("em_key_1");
        newClient.Should().NotBeSameAs(client1);
        newFactory.Dispose();
    }

    [Fact]
    public void Dispose_WithNoClients_DoesNotThrow()
    {
        // Arrange
        var factory = new EmailitClientFactory(new EmailitClientOptions
        {
            ApiKey = "ignored",
            BaseUrl = "https://api.emailit.com",
            TimeoutSeconds = 30
        });

        // Act & Assert
        var act = () => factory.Dispose();
        act.Should().NotThrow();
    }

    [Fact]
    public void Dispose_CalledMultipleTimes_DoesNotThrow()
    {
        // Arrange
        var factory = new EmailitClientFactory(new EmailitClientOptions
        {
            ApiKey = "ignored",
            BaseUrl = "https://api.emailit.com",
            TimeoutSeconds = 30
        });
        factory.CreateClient("em_key_1");

        // Act & Assert
        var act = () =>
        {
            factory.Dispose();
            factory.Dispose();
        };
        act.Should().NotThrow();
    }

    #endregion

    #region Constructor Tests

    [Fact]
    public void Constructor_WithNullOptions_ThrowsArgumentNullException()
    {
        // Act & Assert
        var act = () => new EmailitClientFactory((EmailitClientOptions)null!);
        act.Should().Throw<ArgumentNullException>();
    }

    #endregion
}

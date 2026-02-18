using Emailit.Client.Exceptions;
using Emailit.Client.Models.Webhooks;
using Flurl.Http.Testing;

namespace Emailit.Client.Tests;

public sealed class WebhookTests : IDisposable
{
    private readonly HttpTest _httpTest;
    private readonly EmailitClient _client;

    public WebhookTests()
    {
        _httpTest = new HttpTest();
        _client = new EmailitClient(new EmailitClientOptions
        {
            ApiKey = "em_test_key",
            BaseUrl = "https://api.emailit.com",
            TimeoutSeconds = 30
        });
    }

    public void Dispose()
    {
        _httpTest.Dispose();
    }

    [Fact]
    public async Task CreateWebhookAsync_Success_ReturnsWebhookResponse()
    {
        // Arrange
        _httpTest.RespondWithJson(new
        {
            @object = "webhook",
            id = "wh_abc123",
            name = "Production Webhook",
            url = "https://example.com/webhook",
            all_events = false,
            enabled = true,
            events = new[] { "email.delivered", "email.bounced" },
            created_at = DateTime.UtcNow
        });

        var request = new CreateWebhookRequest
        {
            Name = "Production Webhook",
            Url = "https://example.com/webhook",
            Events = ["email.delivered", "email.bounced"]
        };

        // Act
        var result = await _client.CreateWebhookAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("wh_abc123");
        result.Name.Should().Be("Production Webhook");
        result.Events.Should().Contain("email.delivered");

        _httpTest.ShouldHaveCalled("https://api.emailit.com/v2/webhooks")
            .WithVerb(HttpMethod.Post)
            .Times(1);
    }

    [Fact]
    public async Task CreateWebhookAsync_WithAllEvents_IncludesAllEventsInRequest()
    {
        // Arrange
        _httpTest.RespondWithJson(new
        {
            id = "wh_abc123",
            name = "All Events Webhook",
            url = "https://example.com/webhook",
            all_events = true,
            enabled = true,
            created_at = DateTime.UtcNow
        });

        var request = new CreateWebhookRequest
        {
            Name = "All Events Webhook",
            Url = "https://example.com/webhook",
            AllEvents = true
        };

        // Act
        await _client.CreateWebhookAsync(request);

        // Assert
        _httpTest.ShouldHaveCalled("https://api.emailit.com/v2/webhooks")
            .WithRequestBody("*all_events*")
            .Times(1);
    }

    [Fact]
    public async Task GetWebhookAsync_Success_ReturnsWebhookResponse()
    {
        // Arrange
        _httpTest.RespondWithJson(new
        {
            @object = "webhook",
            id = "wh_abc123",
            name = "My Webhook",
            url = "https://example.com/webhook",
            all_events = false,
            enabled = true,
            events = new[] { "email.delivered" },
            created_at = DateTime.UtcNow
        });

        // Act
        var result = await _client.GetWebhookAsync("wh_abc123");

        // Assert
        result.Id.Should().Be("wh_abc123");
        _httpTest.ShouldHaveCalled("https://api.emailit.com/v2/webhooks/wh_abc123")
            .WithVerb(HttpMethod.Get)
            .Times(1);
    }

    [Fact]
    public async Task GetWebhookAsync_NotFound_ThrowsNotFoundException()
    {
        // Arrange
        _httpTest.RespondWithJson(new { error = "not_found", message = "Webhook not found" }, 404);

        // Act & Assert
        await Assert.ThrowsAsync<EmailitNotFoundException>(
            () => _client.GetWebhookAsync("wh_nonexistent"));
    }

    [Fact]
    public async Task ListWebhooksAsync_Success_ReturnsPaginatedResponse()
    {
        // Arrange
        _httpTest.RespondWithJson(new
        {
            data = new[]
            {
                new { id = "wh_1", name = "Webhook 1", url = "https://example.com/1", enabled = true, created_at = DateTime.UtcNow },
                new { id = "wh_2", name = "Webhook 2", url = "https://example.com/2", enabled = false, created_at = DateTime.UtcNow }
            },
            next_page_url = (string?)null,
            previous_page_url = (string?)null
        });

        // Act
        var result = await _client.ListWebhooksAsync(page: 1, limit: 10);

        // Assert
        result.Data.Should().HaveCount(2);
        _httpTest.ShouldHaveCalled("https://api.emailit.com/v2/webhooks*")
            .WithQueryParam("page", 1)
            .WithQueryParam("limit", 10)
            .Times(1);
    }

    [Fact]
    public async Task UpdateWebhookAsync_Success_ReturnsUpdatedWebhook()
    {
        // Arrange
        _httpTest.RespondWithJson(new
        {
            id = "wh_abc123",
            name = "Updated Webhook",
            url = "https://example.com/new-url",
            enabled = true,
            created_at = DateTime.UtcNow
        });

        var request = new UpdateWebhookRequest
        {
            Name = "Updated Webhook",
            Url = "https://example.com/new-url"
        };

        // Act
        var result = await _client.UpdateWebhookAsync("wh_abc123", request);

        // Assert
        result.Name.Should().Be("Updated Webhook");
        _httpTest.ShouldHaveCalled("https://api.emailit.com/v2/webhooks/wh_abc123")
            .WithVerb(HttpMethod.Post)
            .Times(1);
    }

    [Fact]
    public async Task DeleteWebhookAsync_Success_ReturnsDeleteResponse()
    {
        // Arrange
        _httpTest.RespondWithJson(new
        {
            @object = "webhook",
            id = "wh_abc123",
            name = "My Webhook",
            deleted = true
        });

        // Act
        var result = await _client.DeleteWebhookAsync("wh_abc123");

        // Assert
        result.Deleted.Should().BeTrue();
        result.Id.Should().Be("wh_abc123");
        _httpTest.ShouldHaveCalled("https://api.emailit.com/v2/webhooks/wh_abc123")
            .WithVerb(HttpMethod.Delete)
            .Times(1);
    }

    [Fact]
    public async Task CreateWebhookAsync_Unauthorized_ThrowsAuthenticationException()
    {
        // Arrange
        _httpTest.RespondWithJson(new { error = "unauthorized", message = "Invalid API key" }, 401);

        var request = new CreateWebhookRequest
        {
            Name = "Test",
            Url = "https://example.com/webhook"
        };

        // Act & Assert
        await Assert.ThrowsAsync<EmailitAuthenticationException>(
            () => _client.CreateWebhookAsync(request));
    }
}

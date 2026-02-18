using Emailit.Client.Exceptions;
using Emailit.Client.Models.Events;
using Flurl.Http.Testing;

namespace Emailit.Client.Tests;

public sealed class EventTests : IDisposable
{
    private readonly HttpTest _httpTest;
    private readonly EmailitClient _client;

    public EventTests()
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
    public async Task ListEventsAsync_NoFilters_ReturnsEvents()
    {
        // Arrange
        _httpTest.RespondWithJson(new
        {
            data = new[]
            {
                new { id = "evt_1", type = "email.accepted", created_at = DateTime.UtcNow },
                new { id = "evt_2", type = "email.delivered", created_at = DateTime.UtcNow }
            },
            next_page_url = (string?)null,
            previous_page_url = (string?)null
        });

        // Act
        var result = await _client.ListEventsAsync();

        // Assert
        result.Data.Should().HaveCount(2);
        _httpTest.ShouldHaveCalled("https://api.emailit.com/v2/events*")
            .WithVerb(HttpMethod.Get)
            .Times(1);
    }

    [Fact]
    public async Task ListEventsAsync_WithTypeFilter_SetsQueryParam()
    {
        // Arrange
        _httpTest.RespondWithJson(new
        {
            data = new[] { new { id = "evt_1", type = "email.delivered", created_at = DateTime.UtcNow } },
            next_page_url = (string?)null,
            previous_page_url = (string?)null
        });

        var request = new ListEventsRequest
        {
            Type = "email.delivered,email.bounced"
        };

        // Act
        await _client.ListEventsAsync(request);

        // Assert
        _httpTest.ShouldHaveCalled("https://api.emailit.com/v2/events*")
            .WithQueryParam("type", "email.delivered,email.bounced")
            .Times(1);
    }

    [Fact]
    public async Task ListEventsAsync_WithIncludeData_SetsQueryParam()
    {
        // Arrange
        _httpTest.RespondWithJson(new
        {
            data = new[] { new { id = "evt_1", type = "email.accepted", created_at = DateTime.UtcNow, data = new { } } },
            next_page_url = (string?)null,
            previous_page_url = (string?)null
        });

        var request = new ListEventsRequest
        {
            IncludeData = true
        };

        // Act
        await _client.ListEventsAsync(request);

        // Assert
        _httpTest.ShouldHaveCalled("https://api.emailit.com/v2/events*")
            .WithQueryParam("include_data", "true")
            .Times(1);
    }

    [Fact]
    public async Task ListEventsAsync_WithPagination_SetsPageAndLimit()
    {
        // Arrange
        _httpTest.RespondWithJson(new
        {
            data = Array.Empty<object>(),
            next_page_url = (string?)null,
            previous_page_url = (string?)null
        });

        var request = new ListEventsRequest
        {
            Page = 2,
            Limit = 50
        };

        // Act
        await _client.ListEventsAsync(request);

        // Assert
        _httpTest.ShouldHaveCalled("https://api.emailit.com/v2/events*")
            .WithQueryParam("page", 2)
            .WithQueryParam("limit", 50)
            .Times(1);
    }

    [Fact]
    public async Task GetEventAsync_Success_ReturnsEventResponse()
    {
        // Arrange
        _httpTest.RespondWithJson(new
        {
            @object = "event",
            id = "evt_abc123",
            type = "email.accepted",
            created_at = DateTime.UtcNow
        });

        // Act
        var result = await _client.GetEventAsync("evt_abc123");

        // Assert
        result.Id.Should().Be("evt_abc123");
        result.Type.Should().Be("email.accepted");
        _httpTest.ShouldHaveCalled("https://api.emailit.com/v2/events/evt_abc123")
            .WithVerb(HttpMethod.Get)
            .Times(1);
    }

    [Fact]
    public async Task GetEventAsync_NotFound_ThrowsNotFoundException()
    {
        // Arrange
        _httpTest.RespondWithJson(new { error = "not_found", message = "Event not found" }, 404);

        // Act & Assert
        await Assert.ThrowsAsync<EmailitNotFoundException>(
            () => _client.GetEventAsync("evt_nonexistent"));
    }
}

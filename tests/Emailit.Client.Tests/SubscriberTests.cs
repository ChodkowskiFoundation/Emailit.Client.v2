using Emailit.Client.Models.Subscribers;
using Flurl.Http.Testing;

namespace Emailit.Client.Tests;

public sealed class SubscriberTests : IDisposable
{
    private readonly HttpTest _httpTest;
    private readonly EmailitClient _client;

    public SubscriberTests()
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
    public async Task AddSubscriberAsync_Success_ReturnsSubscriberResponse()
    {
        // Arrange
        _httpTest.RespondWithJson(new
        {
            @object = "subscriber",
            id = "sub_abc123",
            email = "user@example.com",
            first_name = "John",
            last_name = "Doe",
            audience_id = "aud_xyz",
            contact_id = "con_456",
            subscribed = true,
            subscribed_at = DateTime.UtcNow,
            created_at = DateTime.UtcNow
        });

        var request = new AddSubscriberRequest
        {
            Email = "user@example.com",
            FirstName = "John",
            LastName = "Doe"
        };

        // Act
        var result = await _client.AddSubscriberAsync("aud_xyz", request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("sub_abc123");
        result.AudienceId.Should().Be("aud_xyz");
        result.ContactId.Should().Be("con_456");
        result.Subscribed.Should().BeTrue();
        result.SubscribedAt.Should().NotBeNull();

        _httpTest.ShouldHaveCalled("https://api.emailit.com/v2/audiences/aud_xyz/subscribers")
            .WithVerb(HttpMethod.Post)
            .Times(1);
    }

    [Fact]
    public async Task GetSubscriberAsync_Success_ReturnsResponseWithNewFields()
    {
        // Arrange
        var subscribedAt = DateTime.UtcNow.AddDays(-30);
        _httpTest.RespondWithJson(new
        {
            @object = "subscriber",
            id = "sub_abc123",
            email = "user@example.com",
            audience_id = "aud_xyz",
            contact_id = "con_456",
            subscribed = true,
            subscribed_at = subscribedAt,
            unsubscribed_at = (DateTime?)null,
            created_at = DateTime.UtcNow
        });

        // Act
        var result = await _client.GetSubscriberAsync("aud_xyz", "sub_abc123");

        // Assert
        result.AudienceId.Should().Be("aud_xyz");
        result.ContactId.Should().Be("con_456");
        result.Subscribed.Should().BeTrue();
        result.UnsubscribedAt.Should().BeNull();

        _httpTest.ShouldHaveCalled("https://api.emailit.com/v2/audiences/aud_xyz/subscribers/sub_abc123")
            .WithVerb(HttpMethod.Get)
            .Times(1);
    }

    [Fact]
    public async Task ListSubscribersAsync_WithSubscribedFilter_SetsQueryParam()
    {
        // Arrange
        _httpTest.RespondWithJson(new
        {
            data = new[]
            {
                new { id = "sub_1", email = "a@example.com", subscribed = true, created_at = DateTime.UtcNow },
                new { id = "sub_2", email = "b@example.com", subscribed = true, created_at = DateTime.UtcNow }
            },
            next_page_url = (string?)null,
            previous_page_url = (string?)null
        });

        // Act
        var result = await _client.ListSubscribersAsync("aud_xyz", subscribed: true);

        // Assert
        result.Data.Should().HaveCount(2);
        _httpTest.ShouldHaveCalled("https://api.emailit.com/v2/audiences/aud_xyz/subscribers*")
            .WithQueryParam("subscribed", "true")
            .Times(1);
    }

    [Fact]
    public async Task UpdateSubscriberAsync_WithSubscribed_IncludesInRequest()
    {
        // Arrange
        _httpTest.RespondWithJson(new
        {
            @object = "subscriber",
            id = "sub_abc123",
            email = "user@example.com",
            subscribed = false,
            unsubscribed_at = DateTime.UtcNow,
            created_at = DateTime.UtcNow
        });

        var request = new UpdateSubscriberRequest
        {
            Subscribed = false
        };

        // Act
        var result = await _client.UpdateSubscriberAsync("aud_xyz", "sub_abc123", request);

        // Assert
        result.Subscribed.Should().BeFalse();
        _httpTest.ShouldHaveCalled("https://api.emailit.com/v2/audiences/aud_xyz/subscribers/sub_abc123")
            .WithVerb(HttpMethod.Post)
            .WithRequestBody("*subscribed*")
            .Times(1);
    }
}

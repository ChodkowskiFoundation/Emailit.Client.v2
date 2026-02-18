using Emailit.Client.Models.Suppressions;
using Flurl.Http.Testing;

namespace Emailit.Client.Tests;

public sealed class SuppressionTests : IDisposable
{
    private readonly HttpTest _httpTest;
    private readonly EmailitClient _client;

    public SuppressionTests()
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
    public async Task CreateSuppressionAsync_WithKeepUntil_IncludesInRequest()
    {
        // Arrange
        _httpTest.RespondWithJson(new
        {
            @object = "suppression",
            id = "sup_abc123",
            email = "test@example.com",
            type = "bounce",
            reason = "hard bounce",
            keep_until = "2026-12-31T23:59:59Z",
            timestamp = DateTime.UtcNow,
            created_at = DateTime.UtcNow
        });

        var request = new CreateSuppressionRequest
        {
            Email = "test@example.com",
            Type = "bounce",
            Reason = "hard bounce",
            KeepUntil = "2026-12-31T23:59:59Z"
        };

        // Act
        var result = await _client.CreateSuppressionAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("sup_abc123");
        result.KeepUntil.Should().Be("2026-12-31T23:59:59Z");
        result.Timestamp.Should().NotBeNull();

        _httpTest.ShouldHaveCalled("https://api.emailit.com/v2/suppressions")
            .WithVerb(HttpMethod.Post)
            .WithRequestBody("*keep_until*")
            .Times(1);
    }

    [Fact]
    public async Task GetSuppressionAsync_ByEmail_CallsCorrectEndpoint()
    {
        // Arrange
        _httpTest.RespondWithJson(new
        {
            @object = "suppression",
            id = "sup_abc123",
            email = "test@example.com",
            type = "complaint",
            created_at = DateTime.UtcNow
        });

        // Act
        var result = await _client.GetSuppressionAsync("test@example.com");

        // Assert
        result.Email.Should().Be("test@example.com");
        _httpTest.ShouldHaveCalled("https://api.emailit.com/v2/suppressions/test@example.com")
            .WithVerb(HttpMethod.Get)
            .Times(1);
    }

    [Fact]
    public async Task UpdateSuppressionAsync_WithEmailAndKeepUntil_IncludesInRequest()
    {
        // Arrange
        _httpTest.RespondWithJson(new
        {
            @object = "suppression",
            id = "sup_abc123",
            email = "updated@example.com",
            type = "recipient",
            keep_until = "2027-06-15T00:00:00Z",
            created_at = DateTime.UtcNow
        });

        var request = new UpdateSuppressionRequest
        {
            Email = "updated@example.com",
            Type = "recipient",
            KeepUntil = "2027-06-15T00:00:00Z"
        };

        // Act
        var result = await _client.UpdateSuppressionAsync("sup_abc123", request);

        // Assert
        result.KeepUntil.Should().Be("2027-06-15T00:00:00Z");
        _httpTest.ShouldHaveCalled("https://api.emailit.com/v2/suppressions/sup_abc123")
            .WithVerb(HttpMethod.Post)
            .WithRequestBody("*keep_until*")
            .Times(1);
    }

    [Fact]
    public async Task ListSuppressionsAsync_Success_ReturnsPaginatedResponse()
    {
        // Arrange
        _httpTest.RespondWithJson(new
        {
            data = new[]
            {
                new { id = "sup_1", email = "a@example.com", type = "bounce", created_at = DateTime.UtcNow },
                new { id = "sup_2", email = "b@example.com", type = "complaint", created_at = DateTime.UtcNow }
            },
            next_page_url = (string?)null,
            previous_page_url = (string?)null
        });

        // Act
        var result = await _client.ListSuppressionsAsync(page: 1, limit: 50);

        // Assert
        result.Data.Should().HaveCount(2);
        _httpTest.ShouldHaveCalled("https://api.emailit.com/v2/suppressions*")
            .WithQueryParam("page", 1)
            .WithQueryParam("limit", 50)
            .Times(1);
    }

    [Fact]
    public async Task DeleteSuppressionAsync_ByEmail_CallsCorrectEndpoint()
    {
        // Arrange
        _httpTest.RespondWithJson(new
        {
            @object = "suppression",
            id = "sup_abc123",
            deleted = true
        });

        // Act
        var result = await _client.DeleteSuppressionAsync("test@example.com");

        // Assert
        result.Deleted.Should().BeTrue();
        _httpTest.ShouldHaveCalled("https://api.emailit.com/v2/suppressions/test@example.com")
            .WithVerb(HttpMethod.Delete)
            .Times(1);
    }
}

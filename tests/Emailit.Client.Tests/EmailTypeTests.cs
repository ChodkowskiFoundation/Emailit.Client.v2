using Emailit.Client.Models.Emails;
using Flurl.Http.Testing;

namespace Emailit.Client.Tests;

public sealed class EmailTypeTests : IDisposable
{
    private readonly HttpTest _httpTest;
    private readonly EmailitClient _client;

    public EmailTypeTests()
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
    public async Task GetEmailAsync_DeserializesType()
    {
        // Arrange
        _httpTest.RespondWithJson(new
        {
            id = "em_123",
            type = "outbound",
            status = "delivered",
            created_at = DateTime.UtcNow
        });

        // Act
        var result = await _client.GetEmailAsync("em_123");

        // Assert
        result.Type.Should().Be(EmailType.Outbound);
    }

    [Fact]
    public async Task GetEmailAsync_DeserializesInboundType()
    {
        // Arrange
        _httpTest.RespondWithJson(new
        {
            id = "em_456",
            type = "inbound",
            status = "received",
            created_at = DateTime.UtcNow
        });

        // Act
        var result = await _client.GetEmailAsync("em_456");

        // Assert
        result.Type.Should().Be(EmailType.Inbound);
    }

    [Fact]
    public async Task ListEmailsAsync_WithTypeFilter_SendsCorrectQueryParam()
    {
        // Arrange
        _httpTest.RespondWithJson(new
        {
            data = new[] { new { id = "em_1", status = "received", type = "inbound", created_at = DateTime.UtcNow } },
            has_more = false,
            next_cursor = (string?)null
        });

        var request = new ListEmailsRequest { Type = EmailType.Inbound };

        // Act
        await _client.ListEmailsAsync(request);

        // Assert
        _httpTest.ShouldHaveCalled("https://api.emailit.com/v2/emails*")
            .WithQueryParam("type", "inbound")
            .Times(1);
    }

    [Fact]
    public async Task ListEmailsAsync_WithoutTypeFilter_DoesNotSendTypeParam()
    {
        // Arrange
        _httpTest.RespondWithJson(new
        {
            data = Array.Empty<object>(),
            has_more = false,
            next_cursor = (string?)null
        });

        var request = new ListEmailsRequest { Limit = 10 };

        // Act
        await _client.ListEmailsAsync(request);

        // Assert
        _httpTest.ShouldHaveCalled("https://api.emailit.com/v2/emails*")
            .WithoutQueryParam("type")
            .Times(1);
    }
}

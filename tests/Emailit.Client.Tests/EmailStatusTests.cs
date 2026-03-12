using Emailit.Client.Models.Emails;
using Flurl.Http.Testing;

namespace Emailit.Client.Tests;

public sealed class EmailStatusTests : IDisposable
{
    private readonly HttpTest _httpTest;
    private readonly EmailitClient _client;

    public EmailStatusTests()
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
    public void EmailStatus_ContainsAll12Statuses()
    {
        // Arrange
        var expectedStatuses = new[]
        {
            "accepted", "scheduled", "delivered", "bounced", "attempted", "failed",
            "rejected", "loaded", "clicked", "suppressed", "received", "complained"
        };

        var fields = typeof(EmailStatus)
            .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            .Where(f => f.IsLiteral && f.FieldType == typeof(string))
            .Select(f => (string)f.GetRawConstantValue()!)
            .ToArray();

        // Assert
        fields.Should().BeEquivalentTo(expectedStatuses);
    }

    [Fact]
    public async Task ListEmailsAsync_WithEmailStatusConstant_SendsCorrectQueryParam()
    {
        // Arrange
        _httpTest.RespondWithJson(new
        {
            data = new[] { new { id = "em_1", status = "delivered", created_at = DateTime.UtcNow } },
            has_more = false,
            next_cursor = (string?)null
        });

        var request = new ListEmailsRequest { Status = EmailStatus.Delivered };

        // Act
        await _client.ListEmailsAsync(request);

        // Assert
        _httpTest.ShouldHaveCalled("https://api.emailit.com/v2/emails*")
            .WithQueryParam("status", "delivered")
            .Times(1);
    }
}

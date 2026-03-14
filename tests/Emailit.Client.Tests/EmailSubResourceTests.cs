using Emailit.Client.Exceptions;
using Emailit.Client.Models.Emails;
using Flurl.Http.Testing;

namespace Emailit.Client.Tests;

public sealed class EmailSubResourceTests : IDisposable
{
    private readonly HttpTest _httpTest;
    private readonly EmailitClient _client;

    public EmailSubResourceTests()
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

    #region GetEmailMetaAsync

    [Fact]
    public async Task GetEmailMetaAsync_SendsCorrectRequest()
    {
        // Arrange
        _httpTest.RespondWithJson(new
        {
            @object = "email",
            id = "em_123",
            type = "outbound",
            status = "delivered",
            from = "sender@example.com",
            to = new[] { "recipient@example.com" },
            subject = "Test",
            size = 4096,
            created_at = DateTime.UtcNow
        });

        // Act
        var result = await _client.GetEmailMetaAsync("em_123");

        // Assert
        result.Id.Should().Be("em_123");
        result.Type.Should().Be("outbound");
        result.Status.Should().Be("delivered");
        result.From.Should().Be("sender@example.com");
        result.Size.Should().Be(4096);
        _httpTest.ShouldHaveCalled("https://api.emailit.com/v2/emails/em_123/meta")
            .WithVerb(HttpMethod.Get)
            .Times(1);
    }

    [Fact]
    public async Task GetEmailMetaAsync_DeserializesAttachmentInfo()
    {
        // Arrange
        _httpTest.RespondWithJson(new
        {
            id = "em_123",
            status = "delivered",
            created_at = DateTime.UtcNow,
            attachments = new[]
            {
                new { filename = "report.pdf", content_type = "application/pdf", size = 102400, content_id = (string?)null },
                new { filename = "logo.png", content_type = "image/png", size = 2048, content_id = (string?)"logo-cid" }
            }
        });

        // Act
        var result = await _client.GetEmailMetaAsync("em_123");

        // Assert
        result.Attachments.Should().HaveCount(2);
        result.Attachments![0].Filename.Should().Be("report.pdf");
        result.Attachments[0].Size.Should().Be(102400);
        result.Attachments[1].ContentId.Should().Be("logo-cid");
    }

    [Fact]
    public async Task GetEmailMetaAsync_ThrowsNotFound()
    {
        // Arrange
        _httpTest.RespondWithJson(new { error = "not_found", message = "Email not found" }, 404);

        // Act & Assert
        await Assert.ThrowsAsync<EmailitNotFoundException>(
            () => _client.GetEmailMetaAsync("em_nonexistent"));
    }

    #endregion

    #region GetEmailBodyAsync

    [Fact]
    public async Task GetEmailBodyAsync_SendsCorrectRequest()
    {
        // Arrange
        _httpTest.RespondWithJson(new
        {
            @object = "email",
            id = "em_123",
            text = "Plain text content",
            html = "<p>HTML content</p>"
        });

        // Act
        var result = await _client.GetEmailBodyAsync("em_123");

        // Assert
        result.Id.Should().Be("em_123");
        result.Text.Should().Be("Plain text content");
        result.Html.Should().Be("<p>HTML content</p>");
        _httpTest.ShouldHaveCalled("https://api.emailit.com/v2/emails/em_123/body")
            .WithVerb(HttpMethod.Get)
            .Times(1);
    }

    [Fact]
    public async Task GetEmailBodyAsync_HandlesTextOnly()
    {
        // Arrange
        _httpTest.RespondWithJson(new
        {
            id = "em_123",
            text = "Plain text only",
            html = (string?)null
        });

        // Act
        var result = await _client.GetEmailBodyAsync("em_123");

        // Assert
        result.Text.Should().Be("Plain text only");
        result.Html.Should().BeNull();
    }

    [Fact]
    public async Task GetEmailBodyAsync_ThrowsNotFound()
    {
        // Arrange
        _httpTest.RespondWithJson(new { error = "not_found", message = "Email not found" }, 404);

        // Act & Assert
        await Assert.ThrowsAsync<EmailitNotFoundException>(
            () => _client.GetEmailBodyAsync("em_nonexistent"));
    }

    #endregion

    #region GetEmailRawAsync

    [Fact]
    public async Task GetEmailRawAsync_SendsCorrectRequest()
    {
        // Arrange
        var mimeContent = "From: sender@example.com\r\nTo: recipient@example.com\r\nSubject: Test\r\n\r\nBody";
        _httpTest.RespondWithJson(new
        {
            @object = "email",
            id = "em_123",
            raw = mimeContent,
            from = "sender@example.com",
            to = new[] { "recipient@example.com" },
            subject = "Test",
            message_id = "<abc@example.com>",
            size = 1024,
            created_at = DateTime.UtcNow
        });

        // Act
        var result = await _client.GetEmailRawAsync("em_123");

        // Assert
        result.Id.Should().Be("em_123");
        result.Raw.Should().Be(mimeContent);
        result.From.Should().Be("sender@example.com");
        result.MessageId.Should().Be("<abc@example.com>");
        _httpTest.ShouldHaveCalled("https://api.emailit.com/v2/emails/em_123/raw")
            .WithVerb(HttpMethod.Get)
            .Times(1);
    }

    [Fact]
    public async Task GetEmailRawAsync_ThrowsNotFound()
    {
        // Arrange
        _httpTest.RespondWithJson(new { error = "not_found", message = "Email not found" }, 404);

        // Act & Assert
        await Assert.ThrowsAsync<EmailitNotFoundException>(
            () => _client.GetEmailRawAsync("em_nonexistent"));
    }

    #endregion

    #region GetEmailAttachmentsAsync

    [Fact]
    public async Task GetEmailAttachmentsAsync_SendsCorrectRequest()
    {
        // Arrange
        _httpTest.RespondWithJson(new
        {
            @object = "email",
            id = "em_123",
            attachments = new[]
            {
                new { filename = "report.pdf", content = "base64data==", content_type = "application/pdf" }
            }
        });

        // Act
        var result = await _client.GetEmailAttachmentsAsync("em_123");

        // Assert
        result.Id.Should().Be("em_123");
        result.Attachments.Should().HaveCount(1);
        result.Attachments[0].Filename.Should().Be("report.pdf");
        result.Attachments[0].Content.Should().Be("base64data==");
        _httpTest.ShouldHaveCalled("https://api.emailit.com/v2/emails/em_123/attachments")
            .WithVerb(HttpMethod.Get)
            .Times(1);
    }

    [Fact]
    public async Task GetEmailAttachmentsAsync_EmptyAttachments()
    {
        // Arrange
        _httpTest.RespondWithJson(new
        {
            id = "em_123",
            attachments = Array.Empty<object>()
        });

        // Act
        var result = await _client.GetEmailAttachmentsAsync("em_123");

        // Assert
        result.Attachments.Should().BeEmpty();
    }

    [Fact]
    public async Task GetEmailAttachmentsAsync_ThrowsNotFound()
    {
        // Arrange
        _httpTest.RespondWithJson(new { error = "not_found", message = "Email not found" }, 404);

        // Act & Assert
        await Assert.ThrowsAsync<EmailitNotFoundException>(
            () => _client.GetEmailAttachmentsAsync("em_nonexistent"));
    }

    #endregion
}

using Emailit.Client.Exceptions;
using Emailit.Client.Models.Emails;
using Flurl.Http.Testing;

namespace Emailit.Client.Tests;

public sealed class EmailitClientTests : IDisposable
{
    private readonly HttpTest _httpTest;
    private readonly IEmailitClient _client;

    public EmailitClientTests()
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

    #region SendEmailAsync Tests

    [Fact]
    public async Task SendEmailAsync_Success_ReturnsEmailResponse()
    {
        // Arrange
        var expectedResponse = new
        {
            @object = "email",
            id = "em_123",
            status = "queued",
            created_at = DateTime.UtcNow
        };

        _httpTest.RespondWithJson(expectedResponse, 200, new
        {
            ratelimit_limit = "2",
            ratelimit_remaining = "1",
            ratelimit_daily_limit = "5000",
            ratelimit_daily_remaining = "4999"
        });

        var request = new SendEmailRequest
        {
            From = "sender@example.com",
            To = ["recipient@example.com"],
            Subject = "Test Subject",
            Html = "<p>Test body</p>"
        };

        // Act
        var result = await _client.SendEmailAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("em_123");
        result.Status.Should().Be("queued");
        result.RateLimitInfo.Should().NotBeNull();
        result.RateLimitInfo!.DailyRemaining.Should().Be(4999);

        _httpTest.ShouldHaveCalled("https://api.emailit.com/v2/emails")
            .WithVerb(HttpMethod.Post)
            .WithHeader("Authorization", "Bearer em_test_key")
            .Times(1);
    }

    [Fact]
    public async Task SendEmailAsync_WithIdempotencyKey_SetsHeader()
    {
        // Arrange
        _httpTest.RespondWithJson(new { id = "em_123", status = "queued", created_at = DateTime.UtcNow });

        var request = new SendEmailRequest
        {
            From = "sender@example.com",
            To = ["recipient@example.com"],
            Subject = "Test",
            Html = "<p>Test</p>"
        };

        // Act
        await _client.SendEmailAsync(request, idempotencyKey: "unique-key-123");

        // Assert
        _httpTest.ShouldHaveCalled("https://api.emailit.com/v2/emails")
            .WithHeader("Idempotency-Key", "unique-key-123")
            .Times(1);
    }

    [Fact]
    public async Task SendEmailAsync_WithScheduledAt_IncludesInRequest()
    {
        // Arrange
        _httpTest.RespondWithJson(new { id = "em_123", status = "scheduled", created_at = DateTime.UtcNow });

        var request = new SendEmailRequest
        {
            From = "sender@example.com",
            To = ["recipient@example.com"],
            Subject = "Test",
            Html = "<p>Test</p>",
            ScheduledAt = "2024-12-25T10:00:00Z"
        };

        // Act
        var result = await _client.SendEmailAsync(request);

        // Assert
        result.Status.Should().Be("scheduled");
        _httpTest.ShouldHaveCalled("https://api.emailit.com/v2/emails")
            .WithRequestBody("*scheduled_at*")
            .Times(1);
    }

    [Fact]
    public async Task SendEmailAsync_WithAttachments_IncludesInRequest()
    {
        // Arrange
        _httpTest.RespondWithJson(new { id = "em_123", status = "queued", created_at = DateTime.UtcNow });

        var request = new SendEmailRequest
        {
            From = "sender@example.com",
            To = ["recipient@example.com"],
            Subject = "Test",
            Html = "<p>Test</p>",
            Attachments =
            [
                new EmailAttachment
                {
                    Filename = "test.pdf",
                    Content = "base64content",
                    ContentType = "application/pdf"
                }
            ]
        };

        // Act
        await _client.SendEmailAsync(request);

        // Assert
        _httpTest.ShouldHaveCalled("https://api.emailit.com/v2/emails")
            .WithRequestBody("*attachments*")
            .WithRequestBody("*test.pdf*")
            .Times(1);
    }

    [Fact]
    public async Task SendEmailAsync_RateLimitExceeded_ThrowsRateLimitException()
    {
        // Arrange
        _httpTest.RespondWithJson(new { error = "rate_limit_exceeded", message = "Too many requests" }, 429,
            new { ratelimit_remaining = "0", ratelimit_daily_remaining = "100" });

        var request = new SendEmailRequest
        {
            From = "sender@example.com",
            To = ["recipient@example.com"],
            Subject = "Test",
            Html = "<p>Test</p>"
        };

        // Act & Assert
        await Assert.ThrowsAsync<RateLimitExceededException>(
            () => _client.SendEmailAsync(request));
    }

    [Fact]
    public async Task SendEmailAsync_DailyLimitExceeded_ThrowsDailyLimitException()
    {
        // Arrange
        _httpTest.RespondWithJson(new { error = "daily_limit_exceeded", message = "Daily limit reached" }, 429,
            new { ratelimit_remaining = "0", ratelimit_daily_remaining = "0", ratelimit_daily_limit = "5000" });

        var request = new SendEmailRequest
        {
            From = "sender@example.com",
            To = ["recipient@example.com"],
            Subject = "Test",
            Html = "<p>Test</p>"
        };

        // Act & Assert
        await Assert.ThrowsAsync<DailyLimitExceededException>(
            () => _client.SendEmailAsync(request));
    }

    [Fact]
    public async Task SendEmailAsync_Unauthorized_ThrowsAuthenticationException()
    {
        // Arrange
        _httpTest.RespondWithJson(new { error = "unauthorized", message = "Invalid API key" }, 401);

        var request = new SendEmailRequest
        {
            From = "sender@example.com",
            To = ["recipient@example.com"],
            Subject = "Test",
            Html = "<p>Test</p>"
        };

        // Act & Assert
        await Assert.ThrowsAsync<EmailitAuthenticationException>(
            () => _client.SendEmailAsync(request));
    }

    [Fact]
    public async Task SendEmailAsync_Forbidden_ThrowsEmailitException()
    {
        // Arrange
        _httpTest.RespondWithJson(new { error = "forbidden", message = "Access denied" }, 403);

        var request = new SendEmailRequest
        {
            From = "sender@example.com",
            To = ["recipient@example.com"],
            Subject = "Test",
            Html = "<p>Test</p>"
        };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<EmailitException>(
            () => _client.SendEmailAsync(request));
        ex.StatusCode.Should().Be(403);
    }

    [Fact]
    public async Task SendEmailAsync_ServerError_ThrowsEmailitException()
    {
        // Arrange
        _httpTest.RespondWithJson(new { error = "internal_error", message = "Something went wrong" }, 500);

        var request = new SendEmailRequest
        {
            From = "sender@example.com",
            To = ["recipient@example.com"],
            Subject = "Test",
            Html = "<p>Test</p>"
        };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<EmailitException>(
            () => _client.SendEmailAsync(request));
        ex.Message.Should().Contain("Server error");
    }

    [Fact]
    public async Task SendEmailAsync_ValidationError_ThrowsValidationException()
    {
        // Arrange
        _httpTest.RespondWithJson(new
        {
            error = "validation_error",
            message = "Validation failed",
            errors = new { to = new[] { "Invalid email format" } }
        }, 400);

        var request = new SendEmailRequest
        {
            From = "sender@example.com",
            To = ["invalid-email"],
            Subject = "Test",
            Html = "<p>Test</p>"
        };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<EmailitValidationException>(
            () => _client.SendEmailAsync(request));

        ex.ValidationErrors.Should().ContainKey("to");
    }

    #endregion

    #region GetEmailAsync Tests

    [Fact]
    public async Task GetEmailAsync_Success_ReturnsEmailResponse()
    {
        // Arrange
        _httpTest.RespondWithJson(new
        {
            id = "em_123",
            status = "delivered",
            from = "sender@example.com",
            to = new[] { "recipient@example.com" },
            subject = "Test",
            created_at = DateTime.UtcNow,
            delivered_at = DateTime.UtcNow
        });

        // Act
        var result = await _client.GetEmailAsync("em_123");

        // Assert
        result.Id.Should().Be("em_123");
        result.Status.Should().Be("delivered");
        _httpTest.ShouldHaveCalled("https://api.emailit.com/v2/emails/em_123")
            .WithVerb(HttpMethod.Get)
            .Times(1);
    }

    [Fact]
    public async Task GetEmailAsync_NotFound_ThrowsNotFoundException()
    {
        // Arrange
        _httpTest.RespondWithJson(new { error = "not_found", message = "Email not found" }, 404);

        // Act & Assert
        await Assert.ThrowsAsync<EmailitNotFoundException>(
            () => _client.GetEmailAsync("em_nonexistent"));
    }

    #endregion

    #region CancelEmailAsync Tests

    [Fact]
    public async Task CancelEmailAsync_Success_ReturnsTrue()
    {
        // Arrange
        _httpTest.RespondWithJson(new { id = "em_123", cancelled = true });

        // Act
        var result = await _client.CancelEmailAsync("em_123");

        // Assert
        result.Should().BeTrue();
        _httpTest.ShouldHaveCalled("https://api.emailit.com/v2/emails/em_123/cancel")
            .WithVerb(HttpMethod.Post)
            .Times(1);
    }

    [Fact]
    public async Task CancelEmailAsync_Failed_ReturnsFalse()
    {
        // Arrange
        _httpTest.RespondWithJson(new { error = "cannot_cancel", message = "Email already sent" }, 400);

        // Act
        var result = await _client.CancelEmailAsync("em_123");

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region ResendEmailAsync Tests

    [Fact]
    public async Task ResendEmailAsync_Success_CallsResendEndpoint()
    {
        // Arrange
        _httpTest.RespondWithJson(new { id = "em_456", status = "queued", created_at = DateTime.UtcNow });

        // Act
        var result = await _client.ResendEmailAsync("em_123");

        // Assert
        result.Id.Should().Be("em_456");
        _httpTest.ShouldHaveCalled("https://api.emailit.com/v2/emails/em_123/resend")
            .WithVerb(HttpMethod.Post)
            .Times(1);
    }

#pragma warning disable CS0618
    [Fact]
    public async Task RetryEmailAsync_DelegatesToResendEmailAsync()
    {
        // Arrange
        _httpTest.RespondWithJson(new { id = "em_456", status = "queued", created_at = DateTime.UtcNow });

        // Act
        var result = await _client.RetryEmailAsync("em_123");

        // Assert — should call /resend (not /retry)
        result.Id.Should().Be("em_456");
        _httpTest.ShouldHaveCalled("https://api.emailit.com/v2/emails/em_123/resend")
            .WithVerb(HttpMethod.Post)
            .Times(1);
    }
#pragma warning restore CS0618

    #endregion

    #region SendEmailAsync Template Field Tests

    [Fact]
    public async Task SendEmailAsync_WithTemplate_SerializesAsTemplateNotTemplateId()
    {
        // Arrange
        _httpTest.RespondWithJson(new { id = "em_123", status = "queued", created_at = DateTime.UtcNow });

        var request = new SendEmailRequest
        {
            From = "sender@example.com",
            To = ["recipient@example.com"],
            Subject = "Test",
            TemplateId = "tmpl_abc"
        };

        // Act
        await _client.SendEmailAsync(request);

        // Assert — JSON field must be "template", not "template_id"
        _httpTest.ShouldHaveCalled("https://api.emailit.com/v2/emails")
            .WithRequestBody("*\"template\":\"tmpl_abc\"*")
            .Times(1);
    }

    #endregion

    #region ListEmailsAsync Tests

    [Fact]
    public async Task ListEmailsAsync_NoFilters_CallsCorrectEndpoint()
    {
        // Arrange
        _httpTest.RespondWithJson(new
        {
            data = new[]
            {
                new { id = "em_1", status = "delivered", created_at = DateTime.UtcNow },
                new { id = "em_2", status = "queued", created_at = DateTime.UtcNow }
            },
            has_more = false,
            next_cursor = (string?)null
        });

        // Act
        var result = await _client.ListEmailsAsync();

        // Assert
        result.Data.Should().HaveCount(2);
        result.HasMore.Should().BeFalse();
        result.NextCursor.Should().BeNull();
        _httpTest.ShouldHaveCalled("https://api.emailit.com/v2/emails")
            .WithVerb(HttpMethod.Get)
            .Times(1);
    }

    [Fact]
    public async Task ListEmailsAsync_WithFilters_SetsQueryParams()
    {
        // Arrange
        _httpTest.RespondWithJson(new
        {
            data = new[] { new { id = "em_1", status = "delivered", created_at = DateTime.UtcNow } },
            has_more = false,
            next_cursor = (string?)null
        });

        var listRequest = new ListEmailsRequest
        {
            Limit = 10,
            Status = "delivered",
            From = "sender@example.com",
            Tag = "newsletter"
        };

        // Act
        await _client.ListEmailsAsync(listRequest);

        // Assert
        _httpTest.ShouldHaveCalled("https://api.emailit.com/v2/emails*")
            .WithQueryParam("limit", 10)
            .WithQueryParam("status", "delivered")
            .WithQueryParam("from", "sender@example.com")
            .WithQueryParam("tag", "newsletter")
            .Times(1);
    }

    [Fact]
    public async Task ListEmailsAsync_WithCursor_SendsAfterParam()
    {
        // Arrange
        _httpTest.RespondWithJson(new
        {
            data = new[] { new { id = "em_3", status = "sent", created_at = DateTime.UtcNow } },
            has_more = true,
            next_cursor = "cursor_page3"
        });

        var listRequest = new ListEmailsRequest
        {
            After = "cursor_page2",
            Limit = 25
        };

        // Act
        var result = await _client.ListEmailsAsync(listRequest);

        // Assert
        result.HasMore.Should().BeTrue();
        result.NextCursor.Should().Be("cursor_page3");
        _httpTest.ShouldHaveCalled("https://api.emailit.com/v2/emails*")
            .WithQueryParam("after", "cursor_page2")
            .Times(1);
    }

    [Fact]
    public async Task ListEmailsAsync_WithDateFilters_SetsQueryParams()
    {
        // Arrange
        _httpTest.RespondWithJson(new
        {
            data = Array.Empty<object>(),
            has_more = false,
            next_cursor = (string?)null
        });

        var listRequest = new ListEmailsRequest
        {
            CreatedAfter = "2024-01-01T00:00:00Z",
            CreatedBefore = "2024-12-31T23:59:59Z"
        };

        // Act
        await _client.ListEmailsAsync(listRequest);

        // Assert
        _httpTest.ShouldHaveCalled("https://api.emailit.com/v2/emails*")
            .WithQueryParam("created_after", "2024-01-01T00:00:00Z")
            .WithQueryParam("created_before", "2024-12-31T23:59:59Z")
            .Times(1);
    }

    #endregion

    #region Domain Tests

    [Fact]
    public async Task ListDomainsAsync_Success_ReturnsPaginatedResponse()
    {
        // Arrange
        _httpTest.RespondWithJson(new
        {
            data = new[]
            {
                new { id = "dom_1", name = "example.com", status = "verified" },
                new { id = "dom_2", name = "test.com", status = "pending" }
            },
            next_page_url = "https://api.emailit.com/v2/domains?page=2",
            previous_page_url = (string?)null
        });

        // Act
        var result = await _client.ListDomainsAsync(page: 1, limit: 10);

        // Assert
        result.Data.Should().HaveCount(2);
        result.HasNextPage.Should().BeTrue();
        result.HasPreviousPage.Should().BeFalse();
    }

    #endregion

    #region ExportVerificationResultsAsync Tests

    [Fact]
    public async Task ExportVerificationResultsAsync_Unauthorized_ThrowsAuthenticationException()
    {
        // Arrange
        _httpTest.RespondWithJson(new { error = "unauthorized", message = "Invalid API key" }, 401);

        // Act & Assert
        await Assert.ThrowsAsync<EmailitAuthenticationException>(
            () => _client.ExportVerificationResultsAsync("list_123"));
    }

    [Fact]
    public async Task ExportVerificationResultsAsync_RateLimited_ThrowsRateLimitException()
    {
        // Arrange
        _httpTest.RespondWithJson(new { error = "rate_limit_exceeded" }, 429, new
        {
            ratelimit_remaining = "0",
            ratelimit_daily_remaining = "100"
        });

        // Act & Assert
        await Assert.ThrowsAsync<RateLimitExceededException>(
            () => _client.ExportVerificationResultsAsync("list_123"));
    }

    #endregion

    #region TestConnectionAsync Tests

    [Fact]
    public async Task TestConnectionAsync_Success_ReturnsRateLimitInfo()
    {
        // Arrange
        _httpTest.RespondWithJson(new { data = Array.Empty<object>() }, 200, new
        {
            ratelimit_limit = "2",
            ratelimit_remaining = "2",
            ratelimit_daily_limit = "5000",
            ratelimit_daily_remaining = "4500"
        });

        // Act
        var result = await _client.TestConnectionAsync();

        // Assert
        result.Should().NotBeNull();
        result!.Limit.Should().Be(2);
        result.DailyLimit.Should().Be(5000);
        result.DailyRemaining.Should().Be(4500);
    }

    [Fact]
    public async Task TestConnectionAsync_Failed_ReturnsNull()
    {
        // Arrange
        _httpTest.RespondWith("Connection refused", 503);

        // Act
        var result = await _client.TestConnectionAsync();

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region LastRateLimitInfo Tests

    [Fact]
    public async Task LastRateLimitInfo_AfterSendEmail_IsPopulated()
    {
        // Arrange
        _httpTest.RespondWithJson(new { id = "em_1", status = "queued", created_at = DateTime.UtcNow }, 200, new
        {
            ratelimit_limit = "2",
            ratelimit_remaining = "1",
            ratelimit_daily_limit = "5000",
            ratelimit_daily_remaining = "4999"
        });

        // Act
        await _client.SendEmailAsync(new SendEmailRequest
        {
            From = "s@e.com",
            To = ["r@e.com"],
            Subject = "T",
            Html = "<p>T</p>"
        });

        // Assert
        _client.LastRateLimitInfo.Should().NotBeNull();
        _client.LastRateLimitInfo!.Remaining.Should().Be(1);
        _client.LastRateLimitInfo.DailyRemaining.Should().Be(4999);
    }

    [Fact]
    public async Task LastRateLimitInfo_AfterListDomains_IsPopulated()
    {
        // Arrange
        _httpTest.RespondWithJson(new { data = Array.Empty<object>() }, 200, new
        {
            ratelimit_limit = "2",
            ratelimit_remaining = "2",
            ratelimit_daily_limit = "5000",
            ratelimit_daily_remaining = "4800"
        });

        // Act
        await _client.ListDomainsAsync();

        // Assert
        _client.LastRateLimitInfo.Should().NotBeNull();
        _client.LastRateLimitInfo!.DailyRemaining.Should().Be(4800);
    }

    [Fact]
    public async Task LastRateLimitInfo_AfterError429_IsPopulated()
    {
        // Arrange
        _httpTest.RespondWithJson(new { error = "rate_limit_exceeded" }, 429, new
        {
            ratelimit_limit = "2",
            ratelimit_remaining = "0",
            ratelimit_daily_limit = "5000",
            ratelimit_daily_remaining = "100"
        });

        // Act
        try
        {
            await _client.SendEmailAsync(new SendEmailRequest
            {
                From = "s@e.com",
                To = ["r@e.com"],
                Subject = "T",
                Html = "<p>T</p>"
            });
        }
        catch (RateLimitExceededException)
        {
            // expected
        }

        // Assert
        _client.LastRateLimitInfo.Should().NotBeNull();
        _client.LastRateLimitInfo!.Remaining.Should().Be(0);
    }

    [Fact]
    public async Task LastRateLimitInfo_IsOverwrittenByNextRequest()
    {
        // Arrange — first request
        _httpTest.RespondWithJson(new { id = "em_1", status = "queued", created_at = DateTime.UtcNow }, 200, new
        {
            ratelimit_remaining = "2",
            ratelimit_daily_remaining = "4999"
        });

        await _client.SendEmailAsync(new SendEmailRequest
        {
            From = "s@e.com",
            To = ["r@e.com"],
            Subject = "T",
            Html = "<p>T</p>"
        });

        _client.LastRateLimitInfo!.Remaining.Should().Be(2);

        // Arrange — second request
        _httpTest.RespondWithJson(new { id = "em_2", status = "queued", created_at = DateTime.UtcNow }, 200, new
        {
            ratelimit_remaining = "1",
            ratelimit_daily_remaining = "4998"
        });

        // Act
        await _client.SendEmailAsync(new SendEmailRequest
        {
            From = "s@e.com",
            To = ["r@e.com"],
            Subject = "T",
            Html = "<p>T</p>"
        });

        // Assert
        _client.LastRateLimitInfo!.Remaining.Should().Be(1);
        _client.LastRateLimitInfo.DailyRemaining.Should().Be(4998);
    }

    #endregion

    #region Configuration Tests

    [Fact]
    public void Constructor_WithNullOptions_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new EmailitClient((EmailitClientOptions)null!));
    }

    [Fact]
    public void Constructor_WithEmptyApiKey_ThrowsInvalidOperationException()
    {
        // Arrange
        var options = new EmailitClientOptions { ApiKey = "" };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => new EmailitClient(options));
    }

    #endregion

    #region Dispose Tests

    [Fact]
    public void Dispose_DoesNotThrow()
    {
        // Arrange
        var client = new EmailitClient(new EmailitClientOptions
        {
            ApiKey = "em_valid_key",
            BaseUrl = "https://api.emailit.com",
            TimeoutSeconds = 30
        });

        // Act & Assert
        var act = () => client.Dispose();
        act.Should().NotThrow();
    }

    [Fact]
    public void Dispose_CalledMultipleTimes_DoesNotThrow()
    {
        // Arrange
        var client = new EmailitClient(new EmailitClientOptions
        {
            ApiKey = "em_valid_key",
            BaseUrl = "https://api.emailit.com",
            TimeoutSeconds = 30
        });

        // Act & Assert
        var act = () =>
        {
            client.Dispose();
            client.Dispose();
        };
        act.Should().NotThrow();
    }

    #endregion
}

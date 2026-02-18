using Emailit.Client.Models.Verification;
using Flurl.Http.Testing;

namespace Emailit.Client.Tests;

public sealed class VerificationTests : IDisposable
{
    private readonly HttpTest _httpTest;
    private readonly EmailitClient _client;

    public VerificationTests()
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
    public async Task VerifyEmailAsync_Success_ReturnsFullResponse()
    {
        // Arrange
        _httpTest.RespondWithJson(new
        {
            @object = "email_verification",
            id = "ev_abc123",
            email = "user@example.com",
            status = "completed",
            result = "safe",
            score = 95,
            risk = "low",
            mode = "default",
            did_you_mean = (string?)null,
            checks = new
            {
                valid_syntax = true,
                disposable = false,
                role_account = false,
                inbox_full = false,
                deliverable = true,
                disabled = false,
                free_email = false,
                gibberish = false,
                catch_all = false,
                smtp_connect = true,
                has_mx_records = true,
                domain_age = 3650
            },
            address = new
            {
                mailbox = "user",
                domain = "example.com",
                suffix = ".com",
                root = "example"
            },
            mx_records = new[]
            {
                new { priority = 10, exchange = "mx1.example.com" },
                new { priority = 20, exchange = "mx2.example.com" }
            }
        });

        var request = new VerifyEmailRequest { Email = "user@example.com" };

        // Act
        var result = await _client.VerifyEmailAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("ev_abc123");
        result.Email.Should().Be("user@example.com");
        result.Status.Should().Be("completed");
        result.Result.Should().Be("safe");
        result.Score.Should().Be(95);
        result.Risk.Should().Be("low");
        result.Mode.Should().Be("default");
        result.DidYouMean.Should().BeNull();

        _httpTest.ShouldHaveCalled("https://api.emailit.com/v2/email-verifications")
            .WithVerb(HttpMethod.Post)
            .Times(1);
    }

    [Fact]
    public async Task VerifyEmailAsync_WithMode_IncludesModeInRequest()
    {
        // Arrange
        _httpTest.RespondWithJson(new
        {
            id = "ev_abc123",
            email = "user@example.com",
            status = "completed",
            result = "safe",
            score = 90,
            mode = "quick"
        });

        var request = new VerifyEmailRequest
        {
            Email = "user@example.com",
            Mode = "quick"
        };

        // Act
        await _client.VerifyEmailAsync(request);

        // Assert
        _httpTest.ShouldHaveCalled("https://api.emailit.com/v2/email-verifications")
            .WithRequestBody("*mode*")
            .Times(1);
    }

    [Fact]
    public async Task VerifyEmailAsync_ResponseIncludesChecks()
    {
        // Arrange
        _httpTest.RespondWithJson(new
        {
            id = "ev_abc123",
            email = "user@example.com",
            result = "risky",
            score = 40,
            checks = new
            {
                valid_syntax = true,
                disposable = true,
                role_account = false,
                free_email = true,
                catch_all = true,
                has_mx_records = true,
                domain_age = 30
            }
        });

        var request = new VerifyEmailRequest { Email = "user@example.com" };

        // Act
        var result = await _client.VerifyEmailAsync(request);

        // Assert
        result.Checks.Should().NotBeNull();
        result.Checks!.ValidSyntax.Should().BeTrue();
        result.Checks.Disposable.Should().BeTrue();
        result.Checks.FreeEmail.Should().BeTrue();
        result.Checks.CatchAll.Should().BeTrue();
        result.Checks.DomainAge.Should().Be(30);
    }

    [Fact]
    public async Task VerifyEmailAsync_ResponseIncludesAddress()
    {
        // Arrange
        _httpTest.RespondWithJson(new
        {
            id = "ev_abc123",
            email = "user@example.co.uk",
            result = "safe",
            score = 90,
            address = new
            {
                mailbox = "user",
                domain = "example.co.uk",
                suffix = ".co.uk",
                root = "example"
            },
            mx_records = new[]
            {
                new { priority = 10, exchange = "mx.example.co.uk" }
            }
        });

        var request = new VerifyEmailRequest { Email = "user@example.co.uk" };

        // Act
        var result = await _client.VerifyEmailAsync(request);

        // Assert
        result.Address.Should().NotBeNull();
        result.Address!.Mailbox.Should().Be("user");
        result.Address.Domain.Should().Be("example.co.uk");
        result.Address.Suffix.Should().Be(".co.uk");
        result.Address.Root.Should().Be("example");
        result.MxRecords.Should().HaveCount(1);
        result.MxRecords![0].Priority.Should().Be(10);
        result.MxRecords[0].Exchange.Should().Be("mx.example.co.uk");
    }
}

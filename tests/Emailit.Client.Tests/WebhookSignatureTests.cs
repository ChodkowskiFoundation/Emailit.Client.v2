using System.Security.Cryptography;
using System.Text;
using Emailit.Client.Webhooks;

namespace Emailit.Client.Tests;

public sealed class WebhookSignatureTests
{
    private const string TestSecret = "whsec_test_secret_123";
    private const string TestPayload = """{"event_id":"evt_abc","type":"email.delivered","data":{"object":{"id":"em_123"}}}""";
    private const string TestTimestamp = "1709312345";

    [Fact]
    public void ComputeSignature_ReturnsCorrectHmac()
    {
        // Arrange — compute expected value independently
        var signedContent = $"{TestTimestamp}.{TestPayload}";
        var keyBytes = Encoding.UTF8.GetBytes(TestSecret);
        var contentBytes = Encoding.UTF8.GetBytes(signedContent);
        var expectedHash = HMACSHA256.HashData(keyBytes, contentBytes);
        var expected = Convert.ToHexStringLower(expectedHash);

        // Act
        var result = WebhookSignatureValidator.ComputeSignature(TestPayload, TestTimestamp, TestSecret);

        // Assert
        result.Should().Be(expected);
        result.Should().MatchRegex("^[0-9a-f]{64}$");
    }

    [Fact]
    public void ValidateSignature_ReturnsTrueForValidSignature()
    {
        // Arrange
        var signature = WebhookSignatureValidator.ComputeSignature(TestPayload, TestTimestamp, TestSecret);

        // Act
        var result = WebhookSignatureValidator.ValidateSignature(TestPayload, signature, TestTimestamp, TestSecret);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ValidateSignature_ReturnsFalseForInvalidSignature()
    {
        // Arrange
        var invalidSignature = new string('a', 64);

        // Act
        var result = WebhookSignatureValidator.ValidateSignature(TestPayload, invalidSignature, TestTimestamp, TestSecret);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ValidateSignature_ReturnsFalseForTamperedPayload()
    {
        // Arrange
        var signature = WebhookSignatureValidator.ComputeSignature(TestPayload, TestTimestamp, TestSecret);
        var tamperedPayload = TestPayload.Replace("em_123", "em_evil");

        // Act
        var result = WebhookSignatureValidator.ValidateSignature(tamperedPayload, signature, TestTimestamp, TestSecret);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ValidateSignature_WithTolerance_ReturnsFalseForExpiredTimestamp()
    {
        // Arrange — timestamp 10 minutes ago
        var oldTimestamp = DateTimeOffset.UtcNow.AddMinutes(-10).ToUnixTimeSeconds().ToString();
        var signature = WebhookSignatureValidator.ComputeSignature(TestPayload, oldTimestamp, TestSecret);

        // Act
        var result = WebhookSignatureValidator.ValidateSignature(
            TestPayload, signature, oldTimestamp, TestSecret,
            clockTolerance: TimeSpan.FromMinutes(5));

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ValidateSignature_WithTolerance_AcceptsRecentTimestamp()
    {
        // Arrange — timestamp 1 minute ago
        var recentTimestamp = DateTimeOffset.UtcNow.AddMinutes(-1).ToUnixTimeSeconds().ToString();
        var signature = WebhookSignatureValidator.ComputeSignature(TestPayload, recentTimestamp, TestSecret);

        // Act
        var result = WebhookSignatureValidator.ValidateSignature(
            TestPayload, signature, recentTimestamp, TestSecret,
            clockTolerance: TimeSpan.FromMinutes(5));

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ValidateSignature_WithTolerance_ThrowsForInvalidTimestamp()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            WebhookSignatureValidator.ValidateSignature(
                TestPayload, "somesig", "not-a-number", TestSecret,
                clockTolerance: TimeSpan.FromMinutes(5)));
    }
}

using Emailit.Client.Models;

namespace Emailit.Client.Tests;

public sealed class RateLimitInfoTests
{
    [Fact]
    public void FromHeaders_WithAllHeaders_ParsesCorrectly()
    {
        // Arrange
        var headers = new Dictionary<string, string>
        {
            ["ratelimit-limit"] = "2",
            ["ratelimit-remaining"] = "1",
            ["ratelimit-daily-limit"] = "5000",
            ["ratelimit-daily-remaining"] = "4500",
            ["retry-after"] = "30"
        };

        // Act
        var result = RateLimitInfo.FromHeaders(headers);

        // Assert
        result.Limit.Should().Be(2);
        result.Remaining.Should().Be(1);
        result.DailyLimit.Should().Be(5000);
        result.DailyRemaining.Should().Be(4500);
        result.RetryAfterSeconds.Should().Be(30);
    }

    [Fact]
    public void FromHeaders_WithMissingHeaders_ReturnsZeros()
    {
        // Arrange
        var headers = new Dictionary<string, string>();

        // Act
        var result = RateLimitInfo.FromHeaders(headers);

        // Assert
        result.Limit.Should().Be(0);
        result.Remaining.Should().Be(0);
        result.DailyLimit.Should().Be(0);
        result.DailyRemaining.Should().Be(0);
        result.RetryAfterSeconds.Should().BeNull();
    }

    [Fact]
    public void FromHeaders_WithInvalidValues_ReturnsZeros()
    {
        // Arrange
        var headers = new Dictionary<string, string>
        {
            ["ratelimit-limit"] = "invalid",
            ["ratelimit-remaining"] = "not-a-number",
            ["ratelimit-daily-limit"] = "",
            ["ratelimit-daily-remaining"] = "abc"
        };

        // Act
        var result = RateLimitInfo.FromHeaders(headers);

        // Assert
        result.Limit.Should().Be(0);
        result.Remaining.Should().Be(0);
        result.DailyLimit.Should().Be(0);
        result.DailyRemaining.Should().Be(0);
    }

    [Fact]
    public void IsDailyLimitReached_WhenDailyRemainingIsZero_ReturnsTrue()
    {
        // Arrange
        var info = new RateLimitInfo
        {
            DailyLimit = 5000,
            DailyRemaining = 0
        };

        // Act & Assert
        info.IsDailyLimitReached.Should().BeTrue();
    }

    [Fact]
    public void IsDailyLimitReached_WhenDailyRemainingIsPositive_ReturnsFalse()
    {
        // Arrange
        var info = new RateLimitInfo
        {
            DailyLimit = 5000,
            DailyRemaining = 100
        };

        // Act & Assert
        info.IsDailyLimitReached.Should().BeFalse();
    }

    [Fact]
    public void IsDailyLimitReached_WhenNoDailyLimit_ReturnsFalse()
    {
        // Arrange
        var info = new RateLimitInfo
        {
            DailyLimit = 0,
            DailyRemaining = 0
        };

        // Act & Assert
        info.IsDailyLimitReached.Should().BeFalse();
    }

    [Fact]
    public void IsRateLimitReached_WhenRemainingIsZero_ReturnsTrue()
    {
        // Arrange
        var info = new RateLimitInfo
        {
            Limit = 2,
            Remaining = 0
        };

        // Act & Assert
        info.IsRateLimitReached.Should().BeTrue();
    }

    [Fact]
    public void IsRateLimitReached_WhenRemainingIsPositive_ReturnsFalse()
    {
        // Arrange
        var info = new RateLimitInfo
        {
            Limit = 2,
            Remaining = 1
        };

        // Act & Assert
        info.IsRateLimitReached.Should().BeFalse();
    }

    [Fact]
    public void FromHeaders_CaseInsensitive_ParsesCorrectly()
    {
        // Arrange
        var headers = new Dictionary<string, string>
        {
            ["RATELIMIT-LIMIT"] = "2",
            ["RateLimit-Remaining"] = "1"
        };

        // Act
        var result = RateLimitInfo.FromHeaders(headers);

        // Assert
        result.Limit.Should().Be(2);
        result.Remaining.Should().Be(1);
    }
}

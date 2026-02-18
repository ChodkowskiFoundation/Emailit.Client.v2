using Emailit.Client.Exceptions;
using Emailit.Client.Models.Contacts;
using Flurl.Http.Testing;

namespace Emailit.Client.Tests;

public sealed class ContactTests : IDisposable
{
    private readonly HttpTest _httpTest;
    private readonly EmailitClient _client;

    public ContactTests()
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
    public async Task CreateContactAsync_Success_ReturnsContactResponse()
    {
        // Arrange
        _httpTest.RespondWithJson(new
        {
            @object = "contact",
            id = "con_abc123",
            email = "john@example.com",
            first_name = "John",
            last_name = "Doe",
            unsubscribed = false,
            created_at = DateTime.UtcNow
        });

        var request = new CreateContactRequest
        {
            Email = "john@example.com",
            FirstName = "John",
            LastName = "Doe"
        };

        // Act
        var result = await _client.CreateContactAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("con_abc123");
        result.Email.Should().Be("john@example.com");
        result.FirstName.Should().Be("John");
        result.Unsubscribed.Should().BeFalse();

        _httpTest.ShouldHaveCalled("https://api.emailit.com/v2/contacts")
            .WithVerb(HttpMethod.Post)
            .Times(1);
    }

    [Fact]
    public async Task CreateContactAsync_WithAudiences_IncludesAudiencesInRequest()
    {
        // Arrange
        _httpTest.RespondWithJson(new
        {
            id = "con_abc123",
            email = "john@example.com",
            unsubscribed = false,
            audiences = new[]
            {
                new { id = "aud_123", name = "Newsletter" }
            },
            created_at = DateTime.UtcNow
        });

        var request = new CreateContactRequest
        {
            Email = "john@example.com",
            Audiences = ["aud_123"]
        };

        // Act
        await _client.CreateContactAsync(request);

        // Assert
        _httpTest.ShouldHaveCalled("https://api.emailit.com/v2/contacts")
            .WithRequestBody("*audiences*")
            .Times(1);
    }

    [Fact]
    public async Task CreateContactAsync_WithUnsubscribed_SetsGlobalUnsubscribe()
    {
        // Arrange
        _httpTest.RespondWithJson(new
        {
            id = "con_abc123",
            email = "john@example.com",
            unsubscribed = true,
            created_at = DateTime.UtcNow
        });

        var request = new CreateContactRequest
        {
            Email = "john@example.com",
            Unsubscribed = true
        };

        // Act
        var result = await _client.CreateContactAsync(request);

        // Assert
        result.Unsubscribed.Should().BeTrue();
        _httpTest.ShouldHaveCalled("https://api.emailit.com/v2/contacts")
            .WithRequestBody("*unsubscribed*")
            .Times(1);
    }

    [Fact]
    public async Task GetContactAsync_ById_ReturnsContactResponse()
    {
        // Arrange
        _httpTest.RespondWithJson(new
        {
            id = "con_abc123",
            email = "john@example.com",
            first_name = "John",
            unsubscribed = false,
            created_at = DateTime.UtcNow
        });

        // Act
        var result = await _client.GetContactAsync("con_abc123");

        // Assert
        result.Id.Should().Be("con_abc123");
        _httpTest.ShouldHaveCalled("https://api.emailit.com/v2/contacts/con_abc123")
            .WithVerb(HttpMethod.Get)
            .Times(1);
    }

    [Fact]
    public async Task GetContactAsync_ByEmail_ReturnsContactResponse()
    {
        // Arrange
        _httpTest.RespondWithJson(new
        {
            id = "con_abc123",
            email = "john@example.com",
            unsubscribed = false,
            created_at = DateTime.UtcNow
        });

        // Act
        var result = await _client.GetContactAsync("john@example.com");

        // Assert
        result.Email.Should().Be("john@example.com");
        _httpTest.ShouldHaveCalled("https://api.emailit.com/v2/contacts/john@example.com")
            .Times(1);
    }

    [Fact]
    public async Task GetContactAsync_NotFound_ThrowsNotFoundException()
    {
        // Arrange
        _httpTest.RespondWithJson(new { error = "not_found", message = "Contact not found" }, 404);

        // Act & Assert
        await Assert.ThrowsAsync<EmailitNotFoundException>(
            () => _client.GetContactAsync("con_nonexistent"));
    }

    [Fact]
    public async Task ListContactsAsync_Success_ReturnsPaginatedResponse()
    {
        // Arrange
        _httpTest.RespondWithJson(new
        {
            data = new[]
            {
                new { id = "con_1", email = "a@example.com", unsubscribed = false, created_at = DateTime.UtcNow },
                new { id = "con_2", email = "b@example.com", unsubscribed = true, created_at = DateTime.UtcNow }
            },
            next_page_url = (string?)null,
            previous_page_url = (string?)null
        });

        // Act
        var result = await _client.ListContactsAsync(page: 1, limit: 10);

        // Assert
        result.Data.Should().HaveCount(2);
        _httpTest.ShouldHaveCalled("https://api.emailit.com/v2/contacts*")
            .WithQueryParam("page", 1)
            .WithQueryParam("limit", 10)
            .Times(1);
    }

    [Fact]
    public async Task UpdateContactAsync_Success_ReturnsUpdatedContact()
    {
        // Arrange
        _httpTest.RespondWithJson(new
        {
            id = "con_abc123",
            email = "john@example.com",
            first_name = "Jane",
            unsubscribed = false,
            created_at = DateTime.UtcNow
        });

        var request = new UpdateContactRequest
        {
            FirstName = "Jane"
        };

        // Act
        var result = await _client.UpdateContactAsync("con_abc123", request);

        // Assert
        result.FirstName.Should().Be("Jane");
        _httpTest.ShouldHaveCalled("https://api.emailit.com/v2/contacts/con_abc123")
            .WithVerb(HttpMethod.Post)
            .Times(1);
    }

    [Fact]
    public async Task DeleteContactAsync_Success_ReturnsDeleteResponse()
    {
        // Arrange
        _httpTest.RespondWithJson(new
        {
            @object = "contact",
            id = "con_abc123",
            email = "john@example.com",
            deleted = true
        });

        // Act
        var result = await _client.DeleteContactAsync("con_abc123");

        // Assert
        result.Deleted.Should().BeTrue();
        _httpTest.ShouldHaveCalled("https://api.emailit.com/v2/contacts/con_abc123")
            .WithVerb(HttpMethod.Delete)
            .Times(1);
    }
}

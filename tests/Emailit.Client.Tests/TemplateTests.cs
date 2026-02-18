using Emailit.Client.Exceptions;
using Emailit.Client.Models.Templates;
using Flurl.Http.Testing;

namespace Emailit.Client.Tests;

public sealed class TemplateTests : IDisposable
{
    private readonly HttpTest _httpTest;
    private readonly EmailitClient _client;

    public TemplateTests()
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
    public async Task CreateTemplateAsync_Success_ReturnsTemplateResponse()
    {
        // Arrange
        _httpTest.RespondWithJson(new
        {
            data = new
            {
                @object = "template",
                id = "tpl_abc123",
                name = "Welcome Email",
                alias = "welcome-email",
                from = "noreply@example.com",
                subject = "Welcome!",
                editor = "html",
                created_at = DateTime.UtcNow
            },
            message = "Template created successfully"
        });

        var request = new CreateTemplateRequest
        {
            Name = "Welcome Email",
            Alias = "welcome-email",
            From = "noreply@example.com",
            Subject = "Welcome!",
            Html = "<h1>Welcome</h1>"
        };

        // Act
        var result = await _client.CreateTemplateAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("tpl_abc123");
        result.Name.Should().Be("Welcome Email");
        result.Alias.Should().Be("welcome-email");
        result.From.Should().Be("noreply@example.com");
        result.Editor.Should().Be("html");

        _httpTest.ShouldHaveCalled("https://api.emailit.com/v2/templates")
            .WithVerb(HttpMethod.Post)
            .Times(1);
    }

    [Fact]
    public async Task GetTemplateAsync_Success_ReturnsTemplateResponse()
    {
        // Arrange
        _httpTest.RespondWithJson(new
        {
            data = new
            {
                @object = "template",
                id = "tpl_abc123",
                name = "Welcome Email",
                alias = "welcome-email",
                subject = "Welcome!",
                html = "<h1>Welcome</h1>",
                editor = "html",
                reply_to = new[] { "support@example.com" },
                published_at = DateTime.UtcNow,
                preview_url = "https://emailit.com/preview/tpl_abc123",
                created_at = DateTime.UtcNow
            }
        });

        // Act
        var result = await _client.GetTemplateAsync("tpl_abc123");

        // Assert
        result.Id.Should().Be("tpl_abc123");
        result.Alias.Should().Be("welcome-email");
        result.ReplyTo.Should().Contain("support@example.com");
        result.PublishedAt.Should().NotBeNull();
        result.PreviewUrl.Should().Be("https://emailit.com/preview/tpl_abc123");

        _httpTest.ShouldHaveCalled("https://api.emailit.com/v2/templates/tpl_abc123")
            .WithVerb(HttpMethod.Get)
            .Times(1);
    }

    [Fact]
    public async Task GetTemplateAsync_NotFound_ThrowsNotFoundException()
    {
        // Arrange
        _httpTest.RespondWithJson(new { error = "not_found", message = "Template not found" }, 404);

        // Act & Assert
        await Assert.ThrowsAsync<EmailitNotFoundException>(
            () => _client.GetTemplateAsync("tpl_nonexistent"));
    }

    [Fact]
    public async Task ListTemplatesAsync_Success_ReturnsPaginatedResponse()
    {
        // Arrange
        _httpTest.RespondWithJson(new
        {
            data = new[]
            {
                new { id = "tpl_1", name = "Template 1", alias = "template-1", editor = "html", created_at = DateTime.UtcNow },
                new { id = "tpl_2", name = "Template 2", alias = "template-2", editor = "tiptap", created_at = DateTime.UtcNow }
            },
            total_records = 2,
            per_page = 25,
            current_page = 1,
            total_pages = 1
        });

        // Act
        var result = await _client.ListTemplatesAsync();

        // Assert
        result.Data.Should().HaveCount(2);
        result.TotalRecords.Should().Be(2);
        result.PerPage.Should().Be(25);
        result.CurrentPage.Should().Be(1);
        result.TotalPages.Should().Be(1);

        _httpTest.ShouldHaveCalled("https://api.emailit.com/v2/templates*")
            .WithQueryParam("page", 1)
            .WithQueryParam("per_page", 25)
            .Times(1);
    }

    [Fact]
    public async Task ListTemplatesAsync_WithFilters_SendsCorrectQueryParams()
    {
        // Arrange
        _httpTest.RespondWithJson(new
        {
            data = new[]
            {
                new { id = "tpl_1", name = "Welcome", alias = "welcome", editor = "html", created_at = DateTime.UtcNow }
            },
            total_records = 1,
            per_page = 10,
            current_page = 1,
            total_pages = 1
        });

        var request = new ListTemplatesRequest
        {
            Page = 1,
            PerPage = 10,
            FilterName = "Welcome",
            FilterEditor = "html",
            Sort = "created_at",
            Order = "desc"
        };

        // Act
        var result = await _client.ListTemplatesAsync(request);

        // Assert
        result.Data.Should().HaveCount(1);
        _httpTest.ShouldHaveCalled("https://api.emailit.com/v2/templates*")
            .WithQueryParam("per_page", 10)
            .WithQueryParam("filter[name]", "Welcome")
            .WithQueryParam("filter[editor]", "html")
            .WithQueryParam("sort", "created_at")
            .WithQueryParam("order", "desc")
            .Times(1);
    }

    [Fact]
    public async Task UpdateTemplateAsync_Success_ReturnsUpdatedTemplate()
    {
        // Arrange
        _httpTest.RespondWithJson(new
        {
            data = new
            {
                id = "tpl_abc123",
                name = "Updated Template",
                alias = "updated-template",
                subject = "Updated Subject",
                editor = "tiptap",
                created_at = DateTime.UtcNow
            },
            message = "Template updated successfully"
        });

        var request = new UpdateTemplateRequest
        {
            Name = "Updated Template",
            Alias = "updated-template",
            Subject = "Updated Subject"
        };

        // Act
        var result = await _client.UpdateTemplateAsync("tpl_abc123", request);

        // Assert
        result.Name.Should().Be("Updated Template");
        result.Alias.Should().Be("updated-template");
        _httpTest.ShouldHaveCalled("https://api.emailit.com/v2/templates/tpl_abc123")
            .WithVerb(HttpMethod.Post)
            .Times(1);
    }

    [Fact]
    public async Task PublishTemplateAsync_Success_ReturnsPublishedTemplate()
    {
        // Arrange
        _httpTest.RespondWithJson(new
        {
            data = new
            {
                id = "tpl_abc123",
                name = "My Template",
                alias = "my-template",
                published_at = DateTime.UtcNow,
                created_at = DateTime.UtcNow
            },
            message = "Template published"
        });

        // Act
        var result = await _client.PublishTemplateAsync("tpl_abc123");

        // Assert
        result.PublishedAt.Should().NotBeNull();
        _httpTest.ShouldHaveCalled("https://api.emailit.com/v2/templates/tpl_abc123/publish")
            .WithVerb(HttpMethod.Post)
            .Times(1);
    }

    [Fact]
    public async Task DeleteTemplateAsync_Success_ReturnsDeleteResponse()
    {
        // Arrange
        _httpTest.RespondWithJson(new
        {
            data = (object?)null,
            message = "Template deleted successfully"
        });

        // Act
        var result = await _client.DeleteTemplateAsync("tpl_abc123");

        // Assert
        result.Data.Should().BeNull();
        result.Message.Should().Be("Template deleted successfully");
        _httpTest.ShouldHaveCalled("https://api.emailit.com/v2/templates/tpl_abc123")
            .WithVerb(HttpMethod.Delete)
            .Times(1);
    }

    [Fact]
    public async Task CreateTemplateAsync_ValidationError_ThrowsValidationException()
    {
        // Arrange
        _httpTest.RespondWithJson(new { error = "validation_error", message = "The alias field is required." }, 400);

        var request = new CreateTemplateRequest
        {
            Name = "Test",
            Alias = ""
        };

        // Act & Assert
        await Assert.ThrowsAsync<EmailitValidationException>(
            () => _client.CreateTemplateAsync(request));
    }
}

# Emailit.Client

A comprehensive .NET client library for the [Emailit API v2](https://emailit.com/docs/api-reference). Supports all API endpoints including emails, domains, audiences, subscribers, templates, suppressions, and email verification.

## Installation

```bash
dotnet add package Emailit.Client
```

## Quick Start

### 1. Configuration via appsettings.json (Recommended)

Add to your `appsettings.json`:

```json
{
  "Emailit": {
    "ApiKey": "em_your_api_key_here",
    "BaseUrl": "https://api.emailit.com",
    "TimeoutSeconds": 30
  }
}
```

Register in `Program.cs`:

```csharp
using Emailit.Client.DependencyInjection;

builder.Services.AddEmailitClient(builder.Configuration);
```

### 2. Configuration via Action

```csharp
builder.Services.AddEmailitClient(options =>
{
    options.ApiKey = "em_your_api_key_here";
    options.TimeoutSeconds = 60;
});
```

### 3. Configuration via API Key Only

```csharp
builder.Services.AddEmailitClient("em_your_api_key_here");
```

### 4. Manual Instantiation

```csharp
var client = new EmailitClient(new EmailitClientOptions
{
    ApiKey = "em_your_api_key_here",
    BaseUrl = "https://api.emailit.com",
    TimeoutSeconds = 30
});
```

### 5. Multi-Tenant (Factory Pattern)

For applications where each tenant has their own API key:

```csharp
// Register factory
builder.Services.AddEmailitClientFactory(options =>
{
    options.BaseUrl = "https://api.emailit.com";
    options.TimeoutSeconds = 30;
});

// Use in service
public class TenantEmailService
{
    private readonly IEmailitClientFactory _factory;

    public TenantEmailService(IEmailitClientFactory factory)
    {
        _factory = factory;
    }

    public async Task SendForTenant(string tenantApiKey, SendEmailRequest request)
    {
        var client = _factory.CreateClient(tenantApiKey);
        await client.SendEmailAsync(request);
    }
}
```

## Usage Examples

### Inject the Client

```csharp
public class EmailService
{
    private readonly IEmailitClient _emailit;

    public EmailService(IEmailitClient emailit)
    {
        _emailit = emailit;
    }
}
```

### Send an Email

```csharp
var response = await _emailit.SendEmailAsync(new SendEmailRequest
{
    From = "sender@yourdomain.com",
    To = ["recipient@example.com"],
    Subject = "Hello from Emailit!",
    Html = "<h1>Welcome!</h1><p>This is a test email.</p>",
    TrackOpens = true,
    TrackClicks = true
});

Console.WriteLine($"Email ID: {response.Id}, Status: {response.Status}");
```

### Send with Template

```csharp
var response = await _emailit.SendEmailAsync(new SendEmailRequest
{
    From = "sender@yourdomain.com",
    To = ["recipient@example.com"],
    Subject = "Welcome {{first_name}}!",
    TemplateId = "tpl_abc123",
    Variables = new Dictionary<string, object>
    {
        ["first_name"] = "John",
        ["company"] = "Acme Inc"
    }
});
```

### Send with Attachments

```csharp
var response = await _emailit.SendEmailAsync(new SendEmailRequest
{
    From = "sender@yourdomain.com",
    To = ["recipient@example.com"],
    Subject = "Your Invoice",
    Html = "<p>Please find your invoice attached.</p>",
    Attachments =
    [
        new EmailAttachment
        {
            Filename = "invoice.pdf",
            Content = Convert.ToBase64String(pdfBytes), // Base64 for files <= 5MB
            ContentType = "application/pdf"
        },
        new EmailAttachment
        {
            Filename = "large-file.zip",
            Url = "https://your-cdn.com/files/large-file.zip", // URL for files > 5MB
            ContentType = "application/zip"
        }
    ]
});
```

### Schedule an Email

```csharp
var response = await _emailit.SendEmailAsync(new SendEmailRequest
{
    From = "sender@yourdomain.com",
    To = ["recipient@example.com"],
    Subject = "Scheduled Email",
    Html = "<p>This was scheduled!</p>",
    ScheduledAt = "2024-12-25T10:00:00Z" // ISO 8601, Unix timestamp, or natural language
});

// Later, cancel if needed
await _emailit.CancelEmailAsync(response.Id);
```

### Use Idempotency Key

```csharp
// Prevents duplicate sends for 24 hours
var response = await _emailit.SendEmailAsync(
    new SendEmailRequest { /* ... */ },
    idempotencyKey: "order-confirmation-12345"
);
```

### Check Email Status

```csharp
var email = await _emailit.GetEmailAsync("em_abc123");
Console.WriteLine($"Status: {email.Status}");
Console.WriteLine($"Delivered: {email.DeliveredAt}");
```

### Verify Email Address

```csharp
var result = await _emailit.VerifyEmailAsync(new VerifyEmailRequest
{
    Email = "test@example.com"
});

Console.WriteLine($"Result: {result.Result}"); // valid, invalid, risky, unknown
Console.WriteLine($"Risk Score: {result.RiskScore}");
Console.WriteLine($"Disposable: {result.IsDisposable}");
```

### Bulk Email Verification

```csharp
// Create verification list
var list = await _emailit.CreateVerificationListAsync(new CreateVerificationListRequest
{
    Name = "Q4 Campaign Contacts",
    Emails = emailAddresses // Up to 100,000 emails
});

// Check progress
var status = await _emailit.GetVerificationListAsync(list.Id);
Console.WriteLine($"Processed: {status.ProcessedCount}/{status.TotalCount}");

// Get results when complete
if (status.Status == "completed")
{
    var results = await _emailit.GetVerificationResultsAsync(list.Id);
    foreach (var result in results.Data)
    {
        Console.WriteLine($"{result.Email}: {result.Result}");
    }
}
```

### Manage Domains

```csharp
// Create domain
var domain = await _emailit.CreateDomainAsync(new CreateDomainRequest
{
    Name = "yourdomain.com",
    FromEmail = "noreply@yourdomain.com"
});

// Verify DNS records
var verified = await _emailit.VerifyDomainAsync(domain.Id);

// List all domains
var domains = await _emailit.ListDomainsAsync(page: 1, limit: 50);
```

### Manage Suppressions

```csharp
// Add to suppression list
await _emailit.CreateSuppressionAsync(new CreateSuppressionRequest
{
    Email = "bounced@example.com",
    Type = "hard_bounce",
    Reason = "Mailbox does not exist"
});

// List suppressions
var suppressions = await _emailit.ListSuppressionsAsync();
```

## Error Handling

```csharp
try
{
    await _emailit.SendEmailAsync(request);
}
catch (EmailitValidationException ex)
{
    // HTTP 400 - Invalid request
    Console.WriteLine($"Validation error: {ex.Message}");
    foreach (var (field, errors) in ex.ValidationErrors ?? new())
    {
        Console.WriteLine($"  {field}: {string.Join(", ", errors)}");
    }
}
catch (EmailitAuthenticationException ex)
{
    // HTTP 401 - Invalid API key
    Console.WriteLine($"Auth error: {ex.Message}");
}
catch (EmailitNotFoundException ex)
{
    // HTTP 404 - Resource not found
    Console.WriteLine($"Not found: {ex.ResourceType} {ex.ResourceId}");
}
catch (RateLimitExceededException ex)
{
    // HTTP 429 - Per-second rate limit exceeded
    Console.WriteLine($"Rate limited. Retry after {ex.RetryAfterSeconds} seconds");
}
catch (DailyLimitExceededException ex)
{
    // HTTP 429 - Daily limit exceeded
    Console.WriteLine($"Daily limit reached. Resets in {ex.TimeUntilReset}");
}
catch (EmailitException ex)
{
    // Other API errors
    Console.WriteLine($"API error ({ex.StatusCode}): {ex.Message}");
}
```

## Rate Limit Information

Rate limit info is available in email responses:

```csharp
var response = await _emailit.SendEmailAsync(request);

Console.WriteLine($"Rate limit: {response.RateLimitInfo?.Remaining}/{response.RateLimitInfo?.Limit} per second");
Console.WriteLine($"Daily remaining: {response.RateLimitInfo?.DailyRemaining}/{response.RateLimitInfo?.DailyLimit}");

if (response.RateLimitInfo?.IsDailyLimitReached == true)
{
    Console.WriteLine("Daily limit reached!");
}
```

## Supported Endpoints

### Emails
| Method | Description |
|--------|-------------|
| `SendEmailAsync` | Send an email (with optional scheduling, attachments, templates) |
| `GetEmailAsync` | Get email details by ID |
| `UpdateScheduledEmailAsync` | Update scheduled email's send time |
| `CancelEmailAsync` | Cancel a scheduled email |
| `RetryEmailAsync` | Retry a failed email |

### Domains
| Method | Description |
|--------|-------------|
| `CreateDomainAsync` | Create a new sending domain |
| `GetDomainAsync` | Get domain details |
| `ListDomainsAsync` | List all domains (paginated) |
| `UpdateDomainAsync` | Update domain settings |
| `VerifyDomainAsync` | Verify domain DNS records |
| `DeleteDomainAsync` | Delete a domain |

### API Keys
| Method | Description |
|--------|-------------|
| `CreateApiKeyAsync` | Create a new API key |
| `GetApiKeyAsync` | Get API key details |
| `ListApiKeysAsync` | List all API keys (paginated) |
| `UpdateApiKeyAsync` | Update API key name |
| `DeleteApiKeyAsync` | Delete an API key |

### Audiences
| Method | Description |
|--------|-------------|
| `CreateAudienceAsync` | Create a new audience |
| `GetAudienceAsync` | Get audience details |
| `ListAudiencesAsync` | List all audiences (paginated) |
| `UpdateAudienceAsync` | Update audience name |
| `DeleteAudienceAsync` | Delete an audience |

### Subscribers
| Method | Description |
|--------|-------------|
| `AddSubscriberAsync` | Add subscriber to audience |
| `GetSubscriberAsync` | Get subscriber details |
| `ListSubscribersAsync` | List subscribers in audience (paginated) |
| `UpdateSubscriberAsync` | Update subscriber details |
| `DeleteSubscriberAsync` | Delete subscriber from audience |

### Templates
| Method | Description |
|--------|-------------|
| `CreateTemplateAsync` | Create a new template |
| `GetTemplateAsync` | Get template details |
| `ListTemplatesAsync` | List all templates (paginated) |
| `UpdateTemplateAsync` | Update template content |
| `PublishTemplateAsync` | Publish a template |
| `DeleteTemplateAsync` | Delete a template |

### Suppressions
| Method | Description |
|--------|-------------|
| `CreateSuppressionAsync` | Add email to suppression list |
| `GetSuppressionAsync` | Get suppression details |
| `ListSuppressionsAsync` | List all suppressions (paginated) |
| `UpdateSuppressionAsync` | Update suppression entry |
| `DeleteSuppressionAsync` | Remove from suppression list |

### Email Verification
| Method | Description |
|--------|-------------|
| `VerifyEmailAsync` | Verify a single email address |
| `CreateVerificationListAsync` | Create bulk verification list (up to 100k emails) |
| `GetVerificationListAsync` | Get verification list status |
| `ListVerificationListsAsync` | List all verification lists |
| `GetVerificationResultsAsync` | Get verification results |
| `ExportVerificationResultsAsync` | Export results to XLSX |

### Utilities
| Method | Description |
|--------|-------------|
| `TestConnectionAsync` | Test API connection and get rate limits |

## Configuration Options

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ApiKey` | string | (required) | Your Emailit API key |
| `BaseUrl` | string | `https://api.emailit.com` | API base URL |
| `TimeoutSeconds` | int | `30` | Request timeout in seconds |

## Requirements

- .NET 10.0+
- Flurl.Http 4.0+

## License

MIT

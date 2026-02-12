# Emailit.Client v2

A comprehensive .NET client library for the [Emailit API v2](https://emailit.com/docs/api-reference). Built on top of [Flurl HTTP](https://flurl.dev/docs/fluent-http/), it supports all API endpoints including emails, domains, audiences, subscribers, templates, suppressions, and email verification.

## Installation

```bash
dotnet add package Emailit.Client.v2
```

**Target frameworks:** .NET 8.0, .NET 9.0, .NET 10.0

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

### 4. Configuration with appsettings + Overrides

```csharp
builder.Services.AddEmailitClient(builder.Configuration, options =>
{
    options.TimeoutSeconds = 60; // Override specific settings
});
```

### 5. Manual Instantiation

```csharp
using var client = new EmailitClient(new EmailitClientOptions
{
    ApiKey = "em_your_api_key_here",
    BaseUrl = "https://api.emailit.com",
    TimeoutSeconds = 30
});
```

### 6. Multi-Tenant (Factory Pattern)

For applications where each tenant has their own API key. Clients are cached per API key and reused to avoid socket exhaustion.

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
    ScheduledAt = "2025-12-25T10:00:00Z" // ISO 8601, Unix timestamp, or natural language
});

// Update the scheduled time
await _emailit.UpdateScheduledEmailAsync(response.Id, new UpdateScheduledEmailRequest
{
    ScheduledAt = "2025-12-26T10:00:00Z"
});

// Or cancel if no longer needed
await _emailit.CancelEmailAsync(response.Id);
```

### Use Idempotency Key

```csharp
// Prevents duplicate sends for 24 hours
var response = await _emailit.SendEmailAsync(
    new SendEmailRequest
    {
        From = "sender@yourdomain.com",
        To = ["recipient@example.com"],
        Subject = "Order Confirmation",
        Html = "<p>Your order has been confirmed.</p>"
    },
    idempotencyKey: "order-confirmation-12345"
);
```

### List and Filter Emails

```csharp
var emails = await _emailit.ListEmailsAsync(new ListEmailsRequest
{
    Limit = 50,
    Status = "delivered",
    From = "sender@yourdomain.com",
    CreatedAfter = "2025-01-01T00:00:00Z"
});

foreach (var email in emails.Data)
{
    Console.WriteLine($"{email.Id}: {email.Subject} — {email.Status}");
}

// Cursor-based pagination
if (emails.HasMore)
{
    var nextPage = await _emailit.ListEmailsAsync(new ListEmailsRequest
    {
        After = emails.NextCursor
    });
}
```

### Check Email Status and Resend

```csharp
var email = await _emailit.GetEmailAsync("em_abc123");
Console.WriteLine($"Status: {email.Status}, Delivered: {email.DeliveredAt}");

// Resend a failed email (creates a new email with a new ID)
if (email.Status == "bounced")
{
    var resent = await _emailit.ResendEmailAsync(email.Id);
    Console.WriteLine($"Resent as: {resent.Id}");
}
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
Console.WriteLine($"Deliverable: {result.IsDeliverable}");
```

### Bulk Email Verification

```csharp
// Create verification list (up to 100,000 emails)
var list = await _emailit.CreateVerificationListAsync(new CreateVerificationListRequest
{
    Name = "Q4 Campaign Contacts",
    Emails = emailAddresses
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

    // Or export to XLSX
    var downloadUrl = await _emailit.ExportVerificationResultsAsync(list.Id);
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
Console.WriteLine($"Status: {verified.Status}");

// Check required DNS records
foreach (var record in verified.DnsRecords ?? [])
{
    Console.WriteLine($"  {record.Type} {record.Name} -> {record.Value} ({record.Status})");
}

// List all domains
var domains = await _emailit.ListDomainsAsync(page: 1, limit: 50);
```

### Manage Audiences and Subscribers

```csharp
// Create an audience
var audience = await _emailit.CreateAudienceAsync(new CreateAudienceRequest
{
    Name = "Newsletter Subscribers"
});

// Add a subscriber
var subscriber = await _emailit.AddSubscriberAsync(audience.Id, new AddSubscriberRequest
{
    Email = "john@example.com",
    FirstName = "John",
    LastName = "Doe",
    CustomFields = new Dictionary<string, object>
    {
        ["plan"] = "premium",
        ["signup_source"] = "website"
    }
});

// List subscribers with pagination
var subscribers = await _emailit.ListSubscribersAsync(audience.Id, page: 1, limit: 50);
foreach (var sub in subscribers.Data)
{
    Console.WriteLine($"{sub.Email} ({sub.Status})");
}

// Update a subscriber
await _emailit.UpdateSubscriberAsync(audience.Id, subscriber.Id, new UpdateSubscriberRequest
{
    FirstName = "Jonathan"
});
```

### Manage Templates

```csharp
// Create a template
var template = await _emailit.CreateTemplateAsync(new CreateTemplateRequest
{
    Name = "Welcome Email",
    Subject = "Welcome, {{first_name}}!",
    Html = "<h1>Hello {{first_name}}</h1><p>Welcome to {{company}}.</p>"
});

// Publish the template
await _emailit.PublishTemplateAsync(template.Id);

// List all templates
var templates = await _emailit.ListTemplatesAsync();
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
foreach (var s in suppressions.Data)
{
    Console.WriteLine($"{s.Email}: {s.Type} — {s.Reason}");
}
```

### Manage API Keys

```csharp
// Create a scoped API key
var apiKey = await _emailit.CreateApiKeyAsync(new CreateApiKeyRequest
{
    Name = "Transactional Service",
    Scope = "sending",
    SendingDomainId = "dom_abc123"
});

Console.WriteLine($"Key: {apiKey.Key}"); // Only available on creation

// List all API keys
var keys = await _emailit.ListApiKeysAsync();
```

## Error Handling

All API errors are mapped to specific exception types under `Emailit.Client.Exceptions`:

```csharp
using Emailit.Client.Exceptions;

try
{
    await _emailit.SendEmailAsync(request);
}
catch (EmailitValidationException ex)
{
    // HTTP 400 — Invalid request
    Console.WriteLine($"Validation error: {ex.Message}");
    foreach (var (field, errors) in ex.ValidationErrors ?? new())
    {
        Console.WriteLine($"  {field}: {string.Join(", ", errors)}");
    }
}
catch (EmailitAuthenticationException)
{
    // HTTP 401 — Invalid or missing API key
}
catch (EmailitNotFoundException ex)
{
    // HTTP 404 — Resource not found
    Console.WriteLine($"Not found: {ex.ResourceType} {ex.ResourceId}");
}
catch (DailyLimitExceededException ex)
{
    // HTTP 429 — Daily limit exceeded
    var resetIn = RateLimitHelper.GetTimeUntilReset();
    Console.WriteLine($"Daily limit reached. Resets in {resetIn}");
}
catch (RateLimitExceededException ex)
{
    // HTTP 429 — Per-second rate limit exceeded
    Console.WriteLine($"Rate limited. Retry after {ex.RateLimitInfo?.RetryAfterSeconds}s");
}
catch (EmailitException ex)
{
    // Other API errors (403, 413, 5xx, etc.)
    Console.WriteLine($"API error ({ex.StatusCode}): {ex.Message}");
}
```

> **Note:** Catch `DailyLimitExceededException` before `RateLimitExceededException` since the former inherits from the latter.

## Rate Limit Information

Rate limit data is automatically parsed from API response headers and available in two ways:

```csharp
// 1. On EmailResponse — populated after SendEmailAsync
var response = await _emailit.SendEmailAsync(request);

Console.WriteLine($"Rate limit: {response.RateLimitInfo?.Remaining}/{response.RateLimitInfo?.Limit} per second");
Console.WriteLine($"Daily: {response.RateLimitInfo?.DailyRemaining}/{response.RateLimitInfo?.DailyLimit}");

// 2. On the client — always updated after any API call (thread-safe)
var info = _emailit.LastRateLimitInfo;

if (info?.IsDailyLimitReached == true)
{
    var resetIn = RateLimitHelper.GetTimeUntilReset(); // time until midnight UTC
    Console.WriteLine($"Daily limit reached! Resets in {resetIn}");
}

if (info?.IsRateLimitReached == true)
{
    await Task.Delay(TimeSpan.FromSeconds(info.RetryAfterSeconds ?? 1));
}
```

### Test Connection

```csharp
var rateLimitInfo = await _emailit.TestConnectionAsync();
if (rateLimitInfo != null)
{
    Console.WriteLine($"Connected! Daily remaining: {rateLimitInfo.DailyRemaining}");
}
```

## Supported Endpoints

### Emails
| Method | Description |
|--------|-------------|
| `SendEmailAsync` | Send an email (with optional scheduling, attachments, templates) |
| `GetEmailAsync` | Get email details by ID |
| `ListEmailsAsync` | List emails with cursor-based pagination and filters |
| `UpdateScheduledEmailAsync` | Update scheduled email's send time |
| `CancelEmailAsync` | Cancel a scheduled email |
| `ResendEmailAsync` | Resend a failed email (creates a new email with a new ID) |

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
| `ExportVerificationResultsAsync` | Export results as XLSX (returns download URL) |

### Utilities
| Method | Description |
|--------|-------------|
| `TestConnectionAsync` | Test API connection and get rate limit info |

## Configuration Options

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ApiKey` | `string` | *(required)* | Your Emailit API key |
| `BaseUrl` | `string` | `https://api.emailit.com` | API base URL |
| `TimeoutSeconds` | `int` | `30` | Request timeout in seconds |

## Requirements

- .NET 8.0 / 9.0 / 10.0
- [Flurl.Http](https://flurl.dev/docs/fluent-http/) 4.0+

## Resources

- [Emailit API Reference](https://emailit.com/docs/api-reference)
- [Flurl HTTP Documentation](https://flurl.dev/docs/fluent-http/)

## License

MIT

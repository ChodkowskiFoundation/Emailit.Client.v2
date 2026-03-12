# Emailit.Client v2

A comprehensive .NET client library for the [Emailit API v2](https://emailit.com/docs/api-reference). Built on top of [Flurl HTTP](https://flurl.dev/docs/fluent-http/), it supports all API endpoints including emails, domains, audiences, subscribers, templates, suppressions, email verification, contacts, events, and webhooks.

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
    Tracking = new EmailTrackingOptions
    {
        Loads = true,
        Clicks = true
    }
});

Console.WriteLine($"Email ID: {response.Id}, Status: {response.Status}");
```

### Send with Template

```csharp
var response = await _emailit.SendEmailAsync(new SendEmailRequest
{
    From = "sender@yourdomain.com",
    To = ["recipient@example.com"],
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

### Send with Custom Headers and Metadata

```csharp
var response = await _emailit.SendEmailAsync(new SendEmailRequest
{
    From = "sender@yourdomain.com",
    To = ["recipient@example.com"],
    Subject = "Order Confirmation",
    Html = "<p>Your order has been confirmed.</p>",
    Headers = new Dictionary<string, string>
    {
        ["X-Custom-Header"] = "custom-value"
    },
    Meta = new Dictionary<string, string>
    {
        ["order_id"] = "ORD-12345",
        ["customer_id"] = "CUS-678"
    }
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
    ScheduledAt = "2026-12-25T10:00:00Z" // ISO 8601, Unix timestamp, or natural language
});

// Update the scheduled time
await _emailit.UpdateScheduledEmailAsync(response.Id, new UpdateScheduledEmailRequest
{
    ScheduledAt = "2026-12-26T10:00:00Z"
});

// Or cancel if no longer needed
var cancelResult = await _emailit.CancelEmailAsync(response.Id);
Console.WriteLine($"Cancel status: {cancelResult.Status}");
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
    Status = EmailStatus.Delivered,
    Type = EmailType.Outbound, // Filter by inbound/outbound
    From = "sender@yourdomain.com",
    CreatedAfter = "2026-01-01T00:00:00Z"
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

### Retry a Failed Email

```csharp
var email = await _emailit.GetEmailAsync("em_abc123");

if (email.Status == EmailStatus.Bounced)
{
    var retried = await _emailit.RetryEmailAsync(email.Id);
    Console.WriteLine($"Retried as: {retried.Id}, Status: {retried.Status}");
}
```

### Email Sub-Resources

Retrieve specific parts of an email without loading the full object:

```csharp
// Metadata only (no body content) — lightweight
var meta = await _emailit.GetEmailMetaAsync("em_abc123");
Console.WriteLine($"Subject: {meta.Subject}, Size: {meta.Size} bytes");
Console.WriteLine($"Type: {meta.Type}"); // "inbound" or "outbound"
foreach (var att in meta.Attachments ?? [])
    Console.WriteLine($"  Attachment: {att.Filename} ({att.Size} bytes)");

// Body only (text + HTML)
var body = await _emailit.GetEmailBodyAsync("em_abc123");
Console.WriteLine($"HTML: {body.Html?.Length ?? 0} chars, Text: {body.Text?.Length ?? 0} chars");

// Full raw MIME message
var raw = await _emailit.GetEmailRawAsync("em_abc123");
Console.WriteLine($"Raw MIME ({raw.Size} bytes): {raw.Raw[..100]}...");

// Attachments with base64 content
var attachments = await _emailit.GetEmailAttachmentsAsync("em_abc123");
foreach (var att in attachments.Attachments)
    Console.WriteLine($"  {att.Filename}: {att.Content?.Length ?? 0} base64 chars");
```

### Verify Email Address

```csharp
var result = await _emailit.VerifyEmailAsync(new VerifyEmailRequest
{
    Email = "test@example.com"
});

Console.WriteLine($"Result: {result.Result}"); // safe, invalid, risky, unknown
Console.WriteLine($"Score: {result.Score}");
Console.WriteLine($"Risk: {result.Risk}");

// Detailed checks
if (result.Checks != null)
{
    Console.WriteLine($"Disposable: {result.Checks.Disposable}");
    Console.WriteLine($"Deliverable: {result.Checks.Deliverable}");
    Console.WriteLine($"Free email: {result.Checks.FreeEmail}");
}

// Parsed address
if (result.Address != null)
{
    Console.WriteLine($"Mailbox: {result.Address.Mailbox}@{result.Address.Domain}");
}
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
    TrackLoads = true,
    TrackClicks = true
});

// Verify DNS records
var verified = await _emailit.VerifyDomainAsync(domain.Id);
Console.WriteLine($"SPF: {verified.SpfStatus}, DKIM: {verified.DkimStatus}");

// Check required DNS records
foreach (var record in verified.DnsRecords ?? [])
{
    Console.WriteLine($"  {record.Type} {record.Name} -> {record.Value} ({record.Status})");
}

// Update domain settings (uses PATCH)
await _emailit.UpdateDomainAsync(domain.Id, new UpdateDomainRequest
{
    TrackLoads = true,
    TrackClicks = false
});

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

// List subscribers with pagination and filter
var subscribers = await _emailit.ListSubscribersAsync(audience.Id, page: 1, limit: 50, subscribed: true);
foreach (var sub in subscribers.Data)
{
    Console.WriteLine($"{sub.Email} (subscribed: {sub.Subscribed})");
}

// Unsubscribe a subscriber
await _emailit.UpdateSubscriberAsync(audience.Id, subscriber.Id, new UpdateSubscriberRequest
{
    Subscribed = false
});
```

### Manage Templates

```csharp
// Create a template
var template = await _emailit.CreateTemplateAsync(new CreateTemplateRequest
{
    Name = "Welcome Email",
    Alias = "welcome-email",
    Subject = "Welcome, {{first_name}}!",
    From = "noreply@yourdomain.com",
    Html = "<h1>Hello {{first_name}}</h1><p>Welcome to {{company}}.</p>",
    Editor = "html"
});

// Publish the template
await _emailit.PublishTemplateAsync(template.Id);

// List templates with filters
var templates = await _emailit.ListTemplatesAsync(new ListTemplatesRequest
{
    PerPage = 10,
    FilterEditor = "html",
    Sort = "created_at",
    Order = "desc"
});
```

### Manage Suppressions

```csharp
// Add to suppression list (with optional expiration)
await _emailit.CreateSuppressionAsync(new CreateSuppressionRequest
{
    Email = "bounced@example.com",
    Type = "bounce",
    Reason = "Mailbox does not exist",
    KeepUntil = "2027-01-01T00:00:00Z" // null for permanent
});

// Look up by email address
var suppression = await _emailit.GetSuppressionAsync("bounced@example.com");

// List suppressions
var suppressions = await _emailit.ListSuppressionsAsync();
foreach (var s in suppressions.Data)
{
    Console.WriteLine($"{s.Email}: {s.Type} — {s.Reason}");
}
```

### Manage Contacts

```csharp
// Create a contact
var contact = await _emailit.CreateContactAsync(new CreateContactRequest
{
    Email = "user@example.com",
    FirstName = "John",
    LastName = "Doe",
    Unsubscribed = false
});

// Get by ID or email
var fetched = await _emailit.GetContactAsync("user@example.com");

// Update a contact (global unsubscribe)
await _emailit.UpdateContactAsync(contact.Id, new UpdateContactRequest
{
    Unsubscribed = true
});

// List all contacts
var contacts = await _emailit.ListContactsAsync(page: 1, limit: 50);
```

### Manage Events

```csharp
// List events with filters
var events = await _emailit.ListEventsAsync(new ListEventsRequest
{
    Page = 1,
    Limit = 50,
    Type = "email.delivered,email.bounced",
    IncludeData = true
});

foreach (var evt in events.Data)
{
    Console.WriteLine($"{evt.Id}: {evt.Type} at {evt.CreatedAt}");
}

// Get a single event
var eventDetail = await _emailit.GetEventAsync("evt_abc123");
```

### Manage Webhooks

```csharp
// Create a webhook
var webhook = await _emailit.CreateWebhookAsync(new CreateWebhookRequest
{
    Name = "Production Webhook",
    Url = "https://yourapp.com/webhooks/emailit",
    Events = ["email.delivered", "email.bounced", "email.complained"]
});

// Update webhook
await _emailit.UpdateWebhookAsync(webhook.Id, new UpdateWebhookRequest
{
    Enabled = false
});

// List all webhooks
var webhooks = await _emailit.ListWebhooksAsync();
```

### Verify Webhook Signatures

Emailit signs every webhook request with HMAC-SHA256. Use `WebhookSignatureValidator` to verify authenticity:

```csharp
using Emailit.Client.Webhooks;

app.MapPost("/webhooks/emailit", async (HttpContext ctx) =>
{
    var body = await new StreamReader(ctx.Request.Body).ReadToEndAsync();
    var signature = ctx.Request.Headers[WebhookHeaders.Signature].ToString();
    var timestamp = ctx.Request.Headers[WebhookHeaders.Timestamp].ToString();

    if (!WebhookSignatureValidator.ValidateSignature(
        body, signature, timestamp, webhookSecret,
        clockTolerance: TimeSpan.FromMinutes(5)))
    {
        return Results.Unauthorized();
    }

    // Process webhook payload...
    return Results.Ok();
});
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
Console.WriteLine($"Last used: {apiKey.LastUsedAt}");

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
| `SendEmailAsync` | Send an email (with optional scheduling, attachments, templates, tracking) |
| `GetEmailAsync` | Get email details by ID |
| `ListEmailsAsync` | List emails with cursor-based pagination and filters |
| `UpdateScheduledEmailAsync` | Update scheduled email's send time |
| `CancelEmailAsync` | Cancel a scheduled email |
| `RetryEmailAsync` | Retry a failed email (creates a new email with a new ID) |
| `GetEmailMetaAsync` | Get email metadata without body content |
| `GetEmailBodyAsync` | Get parsed text and HTML body only |
| `GetEmailRawAsync` | Get full raw MIME message |
| `GetEmailAttachmentsAsync` | Get attachments with base64-encoded content |

### Domains
| Method | Description |
|--------|-------------|
| `CreateDomainAsync` | Create a new sending domain |
| `GetDomainAsync` | Get domain details (includes verification statuses) |
| `ListDomainsAsync` | List all domains (paginated) |
| `UpdateDomainAsync` | Update domain settings (PATCH) |
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
| `ListSubscribersAsync` | List subscribers in audience (paginated, filterable by subscription status) |
| `UpdateSubscriberAsync` | Update subscriber details (including subscription status) |
| `DeleteSubscriberAsync` | Delete subscriber from audience |

### Templates
| Method | Description |
|--------|-------------|
| `CreateTemplateAsync` | Create a new template (with alias, editor type) |
| `GetTemplateAsync` | Get template details |
| `ListTemplatesAsync` | List templates (paginated with filters by name, alias, editor) |
| `UpdateTemplateAsync` | Update template content |
| `PublishTemplateAsync` | Publish a template |
| `DeleteTemplateAsync` | Delete a template |

### Suppressions
| Method | Description |
|--------|-------------|
| `CreateSuppressionAsync` | Add email to suppression list (with optional expiration) |
| `GetSuppressionAsync` | Get suppression details (by ID or email) |
| `ListSuppressionsAsync` | List all suppressions (paginated) |
| `UpdateSuppressionAsync` | Update suppression entry |
| `DeleteSuppressionAsync` | Remove from suppression list (by ID or email) |

### Email Verification
| Method | Description |
|--------|-------------|
| `VerifyEmailAsync` | Verify a single email address (with detailed checks, address parsing, MX records) |
| `CreateVerificationListAsync` | Create bulk verification list (up to 100k emails) |
| `GetVerificationListAsync` | Get verification list status |
| `ListVerificationListsAsync` | List all verification lists |
| `GetVerificationResultsAsync` | Get verification results |
| `ExportVerificationResultsAsync` | Export results as XLSX (returns download URL) |

### Contacts
| Method | Description |
|--------|-------------|
| `CreateContactAsync` | Create a new contact |
| `GetContactAsync` | Get contact by ID or email |
| `ListContactsAsync` | List all contacts (paginated) |
| `UpdateContactAsync` | Update contact (including global unsubscribe) |
| `DeleteContactAsync` | Delete a contact |

### Events
| Method | Description |
|--------|-------------|
| `ListEventsAsync` | List events with optional type filter and pagination |
| `GetEventAsync` | Get event details by ID |

### Webhooks
| Method | Description |
|--------|-------------|
| `CreateWebhookAsync` | Create a new webhook |
| `GetWebhookAsync` | Get webhook details |
| `ListWebhooksAsync` | List all webhooks (paginated) |
| `UpdateWebhookAsync` | Update webhook settings |
| `DeleteWebhookAsync` | Delete a webhook |

### Webhook Signature Verification
| Method | Description |
|--------|-------------|
| `WebhookSignatureValidator.ValidateSignature` | Verify HMAC-SHA256 webhook signature (timing-safe) |
| `WebhookSignatureValidator.ValidateSignature` (with tolerance) | Verify signature with replay attack protection |
| `WebhookSignatureValidator.ComputeSignature` | Compute expected signature for debugging |

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

# Emailit.Client v2

A .NET client library for the [Emailit API v2](https://emailit.com/docs/api-reference), built on top of [Flurl HTTP](https://flurl.dev/docs/fluent-http/).

The client covers emails, domains, API keys, audiences, subscribers, templates, suppressions, email verification, contacts, events, and webhooks.

Current package version prepared in this repository: `2.1.1`.

## Current State

This package is tested against the live production Emailit API, not only against mocked responses.

The client now includes compatibility handling for several production response quirks:

- booleans returned as `true`/`false`, `0`/`1`, or string equivalents
- numbers returned as strings
- timestamps returned in mixed formats, including empty strings
- recipient fields returned as either a single string or an array
- email attachments returned as either `attachments` or `data`
- legacy `ResendEmailAsync` falling back to `RetryEmailAsync` when `/resend` is unavailable

Stable production integration coverage currently includes:

- connection check and rate limits
- domains and API keys
- audiences, subscribers, contacts, and suppressions
- templates
- emails and email sub-resources
- events
- single email verification

Some production endpoints are still inconsistent on the server side and are isolated as unstable integration coverage:

- webhooks
- verification list/results/export endpoints
- retry/resend behavior as standalone production checks

## Installation

```bash
dotnet add package Emailit.Client.v2
```

Target frameworks:

- .NET 8.0
- .NET 9.0
- .NET 10.0

## Configuration

### appsettings.json

```json
{
  "Emailit": {
    "ApiKey": "em_your_api_key_here",
    "BaseUrl": "https://api.emailit.com",
    "TimeoutSeconds": 30
  }
}
```

```csharp
using Emailit.Client.DependencyInjection;

builder.Services.AddEmailitClient(builder.Configuration);
```

### Manual configuration

```csharp
using var client = new EmailitClient(new EmailitClientOptions
{
    ApiKey = "em_your_api_key_here",
    BaseUrl = "https://api.emailit.com",
    TimeoutSeconds = 30
});
```

### Multi-tenant factory

```csharp
builder.Services.AddEmailitClientFactory(options =>
{
    options.BaseUrl = "https://api.emailit.com";
    options.TimeoutSeconds = 30;
});
```

## Quick Usage

### Send an email

```csharp
var response = await client.SendEmailAsync(new SendEmailRequest
{
    From = "sender@yourdomain.com",
    To = ["recipient@example.com"],
    Subject = "Hello from Emailit",
    Html = "<h1>Welcome</h1><p>This is a test email.</p>"
});

Console.WriteLine($"{response.Id} - {response.Status}");
```

### Send with template

```csharp
var response = await client.SendEmailAsync(new SendEmailRequest
{
    From = "sender@yourdomain.com",
    To = ["recipient@example.com"],
    TemplateId = "tpl_abc123",
    Variables = new Dictionary<string, object>
    {
        ["first_name"] = "John"
    }
});
```

### Schedule, update, and cancel

```csharp
var scheduled = await client.SendEmailAsync(new SendEmailRequest
{
    From = "sender@yourdomain.com",
    To = ["recipient@example.com"],
    Subject = "Scheduled email",
    Html = "<p>Scheduled</p>",
    ScheduledAt = "2026-12-25T10:00:00Z"
});

await client.UpdateScheduledEmailAsync(scheduled.Id, new UpdateScheduledEmailRequest
{
    ScheduledAt = "2026-12-26T10:00:00Z"
});

await client.CancelEmailAsync(scheduled.Id);
```

### Email sub-resources

```csharp
var meta = await client.GetEmailMetaAsync("em_abc123");
var body = await client.GetEmailBodyAsync("em_abc123");
var raw = await client.GetEmailRawAsync("em_abc123");
var attachments = await client.GetEmailAttachmentsAsync("em_abc123");
```

### Retry a failed email

```csharp
var retried = await client.RetryEmailAsync("em_failed123");
Console.WriteLine($"{retried.Id} - {retried.Status}");
```

### Verify a single email

```csharp
var verification = await client.VerifyEmailAsync(new VerifyEmailRequest
{
    Email = "user@example.com"
});

Console.WriteLine($"{verification.Email} - {verification.Result}");
```

### Manage domains

```csharp
var domain = await client.CreateDomainAsync(new CreateDomainRequest
{
    Name = "yourdomain.com",
    TrackLoads = true,
    TrackClicks = true
});

await client.UpdateDomainAsync(domain.Id, new UpdateDomainRequest
{
    TrackLoads = true,
    TrackClicks = false
});

await client.VerifyDomainAsync(domain.Id);
```

Note:

- `UpdateDomainAsync` uses `POST` against the production API.

### Manage audiences and subscribers

```csharp
var audience = await client.CreateAudienceAsync(new CreateAudienceRequest
{
    Name = "Newsletter"
});

var subscriber = await client.AddSubscriberAsync(audience.Id, new AddSubscriberRequest
{
    Email = "john@example.com",
    FirstName = "John",
    LastName = "Doe"
});
```

### Manage templates

```csharp
var template = await client.CreateTemplateAsync(new CreateTemplateRequest
{
    Name = "Welcome Email",
    Alias = "welcome-email",
    Subject = "Welcome",
    From = "noreply@yourdomain.com",
    Html = "<h1>Hello</h1>",
    Editor = "html"
});

await client.PublishTemplateAsync(template.Id);
```

### Manage contacts

```csharp
var contact = await client.CreateContactAsync(new CreateContactRequest
{
    Email = "user@example.com",
    FirstName = "John",
    LastName = "Doe"
});

await client.UpdateContactAsync(contact.Id, new UpdateContactRequest
{
    Unsubscribed = true
});
```

### Manage webhooks

```csharp
var webhook = await client.CreateWebhookAsync(new CreateWebhookRequest
{
    Name = "Production Webhook",
    Url = "https://yourapp.com/webhooks/emailit",
    AllEvents = true
});
```

### Verify webhook signatures

```csharp
using Emailit.Client.Webhooks;

var isValid = WebhookSignatureValidator.ValidateSignature(
    payload,
    signature,
    timestamp,
    webhookSecret,
    clockTolerance: TimeSpan.FromMinutes(5));
```

## Error Handling

```csharp
using Emailit.Client.Exceptions;

try
{
    await client.SendEmailAsync(request);
}
catch (EmailitValidationException ex)
{
    Console.WriteLine(ex.Message);
}
catch (EmailitAuthenticationException)
{
}
catch (EmailitNotFoundException)
{
}
catch (DailyLimitExceededException ex)
{
    Console.WriteLine(ex.RateLimitInfo?.DailyRemaining);
}
catch (RateLimitExceededException ex)
{
    Console.WriteLine(ex.RateLimitInfo?.RetryAfterSeconds);
}
catch (EmailitException ex)
{
    Console.WriteLine($"{ex.StatusCode}: {ex.Message}");
}
```

## Rate Limits

Rate limit metadata is exposed on:

- `EmailResponse.RateLimitInfo` after send operations
- `IEmailitClient.LastRateLimitInfo` after any request, including `TestConnectionAsync`

```csharp
var info = await client.TestConnectionAsync();
Console.WriteLine(client.LastRateLimitInfo?.DailyRemaining);
```

## Integration Tests

The repository contains two test projects:

- `tests/Emailit.Client.Tests` - unit tests with mocked HTTP
- `tests/Emailit.Client.IntegrationTests` - production integration tests

### Required environment variables

```powershell
$env:EMAILIT_INTEGRATION_API_KEY = "em_or_secret_key"
$env:EMAILIT_INTEGRATION_DOMAIN = "yourdomain.com"
$env:EMAILIT_INTEGRATION_TO_EMAIL = "you@example.com"
```

Optional:

```powershell
$env:EMAILIT_INTEGRATION_BASE_URL = "https://api.emailit.com"
$env:EMAILIT_INTEGRATION_TIMEOUT_SECONDS = "60"
$env:EMAILIT_INTEGRATION_ENABLE_UNSTABLE = "true"
```

### Run unit tests

```bash
dotnet test tests/Emailit.Client.Tests/Emailit.Client.Tests.csproj -c Release
```

### Run stable production integration tests

```bash
dotnet test tests/Emailit.Client.IntegrationTests/Emailit.Client.IntegrationTests.csproj -c Release --filter "Stability!=Unstable"
```

### Run unstable production integration tests too

```bash
dotnet test tests/Emailit.Client.IntegrationTests/Emailit.Client.IntegrationTests.csproj -c Release
```

## Building the NuGet Package

Only the library project is packaged.

Both test projects are excluded from packaging because their `.csproj` files set:

- `<IsPackable>false</IsPackable>`

Recommended packaging command:

```bash
dotnet pack src/Emailit.Client/Emailit.Client.csproj -c Release -o nupkg
```

This avoids packing the solution and guarantees that integration tests do not enter the NuGet package.

## Supported Endpoints

### Emails

- `SendEmailAsync`
- `GetEmailAsync`
- `ListEmailsAsync`
- `UpdateScheduledEmailAsync`
- `CancelEmailAsync`
- `RetryEmailAsync`
- `ResendEmailAsync` (legacy compatibility path)
- `GetEmailMetaAsync`
- `GetEmailBodyAsync`
- `GetEmailRawAsync`
- `GetEmailAttachmentsAsync`

### Domains

- `CreateDomainAsync`
- `GetDomainAsync`
- `ListDomainsAsync`
- `UpdateDomainAsync`
- `VerifyDomainAsync`
- `DeleteDomainAsync`

### API Keys

- `CreateApiKeyAsync`
- `GetApiKeyAsync`
- `ListApiKeysAsync`
- `UpdateApiKeyAsync`
- `DeleteApiKeyAsync`

### Audiences and Subscribers

- `CreateAudienceAsync`
- `GetAudienceAsync`
- `ListAudiencesAsync`
- `UpdateAudienceAsync`
- `DeleteAudienceAsync`
- `AddSubscriberAsync`
- `GetSubscriberAsync`
- `ListSubscribersAsync`
- `UpdateSubscriberAsync`
- `DeleteSubscriberAsync`

### Templates

- `CreateTemplateAsync`
- `GetTemplateAsync`
- `ListTemplatesAsync`
- `UpdateTemplateAsync`
- `PublishTemplateAsync`
- `DeleteTemplateAsync`

### Suppressions

- `CreateSuppressionAsync`
- `GetSuppressionAsync`
- `ListSuppressionsAsync`
- `UpdateSuppressionAsync`
- `DeleteSuppressionAsync`

### Verification

- `VerifyEmailAsync`
- `CreateVerificationListAsync`
- `GetVerificationListAsync`
- `ListVerificationListsAsync`
- `GetVerificationResultsAsync`
- `ExportVerificationResultsAsync`

### Contacts

- `CreateContactAsync`
- `GetContactAsync`
- `ListContactsAsync`
- `UpdateContactAsync`
- `DeleteContactAsync`

### Events

- `ListEventsAsync`
- `GetEventAsync`

### Webhooks

- `CreateWebhookAsync`
- `GetWebhookAsync`
- `ListWebhooksAsync`
- `UpdateWebhookAsync`
- `DeleteWebhookAsync`

## Resources

- [Emailit API Reference](https://emailit.com/docs/api-reference)
- [Flurl HTTP Documentation](https://flurl.dev/docs/fluent-http/)

## License

MIT

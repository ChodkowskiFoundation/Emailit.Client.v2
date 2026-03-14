# Emailit.Client v2

A .NET client library for the [Emailit API v2](https://emailit.com/docs/api-reference), built on top of [Flurl HTTP](https://flurl.dev/docs/fluent-http/).

The client covers emails, domains, API keys, audiences, subscribers, templates, suppressions, email verification, contacts, events, and webhooks.

Current package version prepared in this repository: `2.1.1`.

## Overview

The library provides:

- async access to the Emailit v2 API through typed request and response models
- dependency injection support and a multi-tenant client factory
- typed exceptions with normalized `ProblemDetails` conversion
- rate limit metadata on responses and on the client instance
- webhook signature verification helpers
- compatibility handling for inconsistent production payload shapes

The client is tolerant of several response variants seen in production, including:

- booleans returned as `true`/`false`, `0`/`1`, or strings
- numeric fields returned as strings
- timestamps returned in mixed formats or as empty strings
- recipient fields returned as either a single string or an array
- attachment payloads returned under either `attachments` or `data`
- legacy resend behavior routed through retry when `/resend` is unavailable

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
    "TimeoutSeconds": 30,
    "ExceptionDetailMode": "Safe",
    "MaxDiagnosticBodyLength": 1024
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
    TimeoutSeconds = 30,
    ExceptionDetailMode = EmailitExceptionDetailMode.Safe,
    MaxDiagnosticBodyLength = 1024
});
```

`ExceptionDetailMode` controls how much context is captured in thrown exceptions:

- `Minimal` - status, error code, rate limits, and transient/retry metadata
- `Safe` - `Minimal` plus request method, request URI without query string, and request ID when available
- `Diagnostic` - `Safe` plus truncated response body and response headers

### Multi-tenant factory

```csharp
builder.Services.AddEmailitClientFactory(options =>
{
    options.BaseUrl = "https://api.emailit.com";
    options.TimeoutSeconds = 30;
});
```

## Common Usage

Typical flow:

1. configure `EmailitClientOptions`
2. inject `IEmailitClient` or create `EmailitClient` manually
3. call a typed async method such as `SendEmailAsync`, `ListDomainsAsync`, or `VerifyEmailAsync`
4. handle typed `EmailitException` subclasses or convert them to `ProblemDetails`

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
    var problem = ex.ToProblemDetails();
    return Results.ValidationProblem(
        ex.ValidationErrors?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) ?? new Dictionary<string, string[]>(),
        detail: problem.Detail,
        title: problem.Title,
        type: problem.Type);
}
catch (RateLimitExceededException ex)
{
    var problem = ex.ToProblemDetails();
    return Results.Problem(
        title: problem.Title,
        type: problem.Type,
        detail: problem.Detail,
        statusCode: problem.Status,
        extensions: new Dictionary<string, object?>(problem.Extensions));
}
catch (EmailitTimeoutException ex) when (ex.IsTransient)
{
    throw;
}
catch (EmailitException ex)
{
    var problem = ex.ToProblemDetails();
    return Results.Problem(
        title: problem.Title,
        type: problem.Type,
        detail: problem.Detail,
        statusCode: problem.Status ?? 502,
        extensions: new Dictionary<string, object?>(problem.Extensions));
}
```

The client classifies failures into distinct exception types so your application can make decisions instead of only logging messages:

- `EmailitValidationException` - invalid request payload
- `EmailitUnprocessableEntityException` - request shape is valid but the current resource state makes the operation invalid
- `EmailitAuthenticationException` / `EmailitAuthorizationException` - credential or permission issues
- `EmailitNotFoundException` / `EmailitConflictException` - resource state issues
- `RateLimitExceededException` / `DailyLimitExceededException` - retry or backoff conditions
- `EmailitTimeoutException` / `EmailitTransportException` - transient network failures
- `EmailitSerializationException` / `EmailitUnexpectedResponseException` - contract mismatches between the API and the client
- `EmailitServerException` - server-side failures returned by Emailit

## Rate Limits

Rate limit metadata is exposed on:

- `EmailResponse.RateLimitInfo` after send operations
- `IEmailitClient.LastRateLimitInfo` after any request, including `TestConnectionAsync`

```csharp
var info = await client.TestConnectionAsync();
Console.WriteLine(client.LastRateLimitInfo?.DailyRemaining);
```

`TestConnectionAsync` throws the same typed exceptions as the rest of the client when the API is unreachable or rejects the request.

## Packaging

Only the library project is packaged.

Both test projects are excluded from packaging because their `.csproj` files set:

- `<IsPackable>false</IsPackable>`

Recommended packaging command:

```bash
dotnet pack src/Emailit.Client/Emailit.Client.csproj -c Release -o nupkg
```

This avoids packing the solution and guarantees that test projects do not enter the NuGet package.

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

## Testing

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

Stable production integration coverage currently includes:

- connection check and rate limits
- domains and API keys
- audiences, subscribers, contacts, and suppressions
- templates
- emails and email sub-resources
- events
- single email verification

Unstable production coverage is kept separate for backend areas that are known to be inconsistent:

- webhooks
- verification list/results/export endpoints
- retry/resend behavior

When running the unstable suite, known backend inconsistencies are treated as observations rather than client regressions when:

- a retryable email candidate cannot be produced within the observation window
- a webhook create response returns an ID that never becomes readable
- verification list creation returns a production `500`

## Resources

- [Emailit API Reference](https://emailit.com/docs/api-reference)
- [Flurl HTTP Documentation](https://flurl.dev/docs/fluent-http/)

## License

MIT

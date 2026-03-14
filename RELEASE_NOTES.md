# Release Notes - Unreleased

## Exception Handling Overhaul

- added richer exception context with request method, sanitized request URI, request ID, transient/retry metadata, rate limits, and optional diagnostic payload capture
- added `EmailitProblemDetails` conversion so consumers can return standardized RFC 7807-style responses without introducing a logger dependency
- added dedicated exception types for authorization, conflicts, transport failures, timeouts, deserialization failures, unexpected response shapes, and server-side failures
- `TestConnectionAsync` now throws typed exceptions instead of silently returning `null`
- added client options to control sensitive diagnostic capture: `ExceptionDetailMode` and `MaxDiagnosticBodyLength`

---

# Release Notes - v2.1.1

## Production Compatibility Hardening

- `UpdateDomainAsync` now uses the production-compatible `POST /v2/domains/{id}` route
- `ResendEmailAsync` now falls back to `RetryEmailAsync` when the legacy `/resend` route is unavailable
- `TestConnectionAsync` now preserves cancellation and updates `LastRateLimitInfo`
- webhook timestamp validation now rejects future timestamps outside tolerance

## Response Deserialization Hardening

- tolerant boolean parsing for `true`/`false`, `0`/`1`, and string equivalents
- tolerant number parsing when production returns numeric values as strings
- tolerant `DateTime` parsing for mixed timestamp formats and empty values
- tolerant recipient parsing when production returns a single string instead of an array
- tolerant attachment parsing when production returns email attachments under `data` instead of `attachments`
- tolerant DNS TTL parsing when production returns TTL as a string

## Integration Tests

- added production integration test project
- split stable integration coverage from unstable endpoint groups
- stable production coverage now validates:
  - connection, domains, and API keys
  - audiences, subscribers, contacts, and suppressions
  - templates
  - emails and email sub-resources
  - events
  - single email verification
- unstable groups remain isolated for webhook endpoints, verification list endpoints, and retry/resend edge cases

---

# Release Notes - v2.1.0

## New Endpoints

### Email Sub-Resources
- `GetEmailMetaAsync` - GET /emails/{id}/meta - Email metadata without body content
- `GetEmailBodyAsync` - GET /emails/{id}/body - Parsed text and HTML body only
- `GetEmailRawAsync` - GET /emails/{id}/raw - Full raw MIME message with metadata
- `GetEmailAttachmentsAsync` - GET /emails/{id}/attachments - Attachment list with base64-encoded content

## New Features

### Webhook Signature Verification
- `WebhookSignatureValidator.ValidateSignature()` - HMAC-SHA256 signature verification with timing-safe comparison
- `WebhookSignatureValidator.ValidateSignature(..., clockTolerance)` - signature verification with replay attack protection
- `WebhookSignatureValidator.ComputeSignature()` - compute expected signature for debugging
- `WebhookHeaders` constants - `X-Emailit-Signature` and `X-Emailit-Timestamp` header names

### Email Status Constants
- `EmailStatus` static class with all 12 API v2 statuses: `Accepted`, `Scheduled`, `Delivered`, `Bounced`, `Attempted`, `Failed`, `Rejected`, `Loaded`, `Clicked`, `Suppressed`, `Received`, `Complained`

### Email Type Constants
- `EmailType` static class: `Inbound`, `Outbound`

## Updated Models
- `EmailResponse` - added `Type` field (inbound/outbound)
- `ListEmailsRequest` - added `Type` filter for inbound/outbound emails
- `TemplateResponse` - added `TotalVersions` field

## New Models
- `EmailMetaResponse` - metadata response for `/emails/{id}/meta`
- `EmailBodyResponse` - body response for `/emails/{id}/body`
- `EmailRawResponse` - raw MIME response for `/emails/{id}/raw`
- `EmailAttachmentsResponse` - attachments response for `/emails/{id}/attachments`
- `EmailAttachmentInfo` - lightweight attachment metadata

---

# Release Notes - v2.0.1

## New Endpoints

### Contacts
- `CreateContactAsync` - create a new contact
- `GetContactAsync` - get contact by ID or email address
- `ListContactsAsync` - list all contacts with pagination
- `UpdateContactAsync` - update contact details, including global unsubscribe
- `DeleteContactAsync` - delete a contact

### Events
- `ListEventsAsync` - list events with type filtering, pagination, and optional data inclusion
- `GetEventAsync` - get event details by ID

### Webhooks
- `CreateWebhookAsync` - create a new webhook with event subscriptions
- `GetWebhookAsync` - get webhook details by ID
- `ListWebhooksAsync` - list all webhooks with pagination
- `UpdateWebhookAsync` - update webhook settings
- `DeleteWebhookAsync` - delete a webhook

## Updated Endpoints

### Emails
- `RetryEmailAsync` - new method replacing `ResendEmailAsync` (`POST /retry` instead of `/resend`)
- `ResendEmailAsync` marked as `[Obsolete]`
- `CancelEmailAsync` now returns `CancelEmailResponse` instead of `bool`

### Domains
- `CreateDomainRequest` removed `FromEmail`, added `TrackLoads` and `TrackClicks`
- `UpdateDomainRequest` added `TrackingKey` and `InboundKey`
- `DomainResponse` added `Uuid`, verification fields, DNS status fields, and `VerifiedAt`

### Templates
- `CreateTemplateRequest` added required `Alias`, optional `From`, `ReplyTo`, and `Editor`
- `UpdateTemplateRequest` added `Alias`, `From`, `ReplyTo`, and `Editor`
- `TemplateResponse` added alias, editor, publish, preview, and version fields
- `ListTemplatesAsync` now uses `ListTemplatesRequest`

### Suppressions
- `CreateSuppressionRequest` added `KeepUntil`
- `UpdateSuppressionRequest` added `Email` and `KeepUntil`
- `SuppressionResponse` added `Timestamp` and `KeepUntil`

### Subscribers
- `SubscriberResponse` added audience and subscription state fields
- `UpdateSubscriberRequest` added `Subscribed`
- `ListSubscribersAsync` added `subscribed` filter

### Email Verification
- `VerifyEmailRequest` added `Mode`
- `EmailVerificationResponse` added detailed scoring, checks, parsed address, and MX records

### API Keys
- `ApiKeyResponse` added `LastUsedAt`

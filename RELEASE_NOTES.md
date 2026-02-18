# Release Notes — v2.0.1

## New Endpoints

### Contacts
- `CreateContactAsync` — Create a new contact
- `GetContactAsync` — Get contact by ID or email address
- `ListContactsAsync` — List all contacts with pagination
- `UpdateContactAsync` — Update contact details, including global unsubscribe
- `DeleteContactAsync` — Delete a contact

### Events
- `ListEventsAsync` — List events with type filtering, pagination, and optional data inclusion
- `GetEventAsync` — Get event details by ID

### Webhooks
- `CreateWebhookAsync` — Create a new webhook with event subscriptions
- `GetWebhookAsync` — Get webhook details by ID
- `ListWebhooksAsync` — List all webhooks with pagination
- `UpdateWebhookAsync` — Update webhook settings
- `DeleteWebhookAsync` — Delete a webhook

## Updated Endpoints

### Emails
- **`RetryEmailAsync`** — New method replacing `ResendEmailAsync` (POST `/retry` instead of `/resend`)
- `ResendEmailAsync` marked as `[Obsolete]`
- `CancelEmailAsync` now returns `CancelEmailResponse` (with `Status` and `Message`) instead of `bool`
- `SendEmailRequest`: replaced `TrackOpens`/`TrackClicks` booleans with `Tracking` object (`EmailTrackingOptions`), replaced `Metadata` with `Meta`, added `Headers` for custom email headers, `Subject` is now optional (template can provide it)
- `EmailResponse`: added `Ids`, `Token`, `MessageId`, `Size`, `Tracking`, `Headers`, `Meta`, `Content`, `Message` fields; removed `Metadata`
- `EmailAttachment`: added `ContentId` and `Encoding` fields

### Domains
- `CreateDomainRequest`: removed `FromEmail`, added `TrackLoads` and `TrackClicks`
- `UpdateDomainAsync` now uses PATCH instead of POST
- `UpdateDomainRequest`: added `TrackingKey` and `InboundKey` fields
- `DomainResponse`: added `Uuid`, `VerificationToken`, `VerificationMethod`, `SpfStatus`, `DkimStatus`, `MxStatus`, `DmarcStatus`, `DkimIdentifierString`, `VerifiedAt`

### Templates
- `CreateTemplateRequest`: added required `Alias`, optional `From`, `ReplyTo`, `Editor` fields
- `UpdateTemplateRequest`: added `Alias`, `From`, `ReplyTo`, `Editor` fields
- `TemplateResponse`: replaced `Version`/`Published` with `Alias`, `From`, `ReplyTo`, `Editor`, `PublishedAt`, `PreviewUrl`, `Versions`
- `ListTemplatesAsync` now accepts `ListTemplatesRequest` with filters (`FilterName`, `FilterAlias`, `FilterEditor`), sorting, and new pagination format (`PerPage`, `TotalRecords`, `CurrentPage`, `TotalPages`)
- `DeleteTemplateResponse` updated to new format with `Data` and `Message`
- Template CRUD responses are now wrapped in `{data: {...}, message: "..."}` — handled transparently by the client

### Suppressions
- `CreateSuppressionRequest`: added `KeepUntil` for temporary suppressions; type values updated to `recipient`, `bounce`, `complaint`, `unsubscribe`
- `UpdateSuppressionRequest`: added `Email` and `KeepUntil` fields
- `SuppressionResponse`: added `Timestamp` and `KeepUntil` fields
- Suppressions can now be looked up by email address (in addition to ID)

### Subscribers
- `SubscriberResponse`: added `AudienceId`, `ContactId`, `Subscribed`, `SubscribedAt`, `UnsubscribedAt`; removed `Status`
- `UpdateSubscriberRequest`: added `Subscribed` field for managing subscription status
- `ListSubscribersAsync`: added `subscribed` filter parameter

### Email Verification
- `VerifyEmailRequest`: added `Mode` field
- `EmailVerificationResponse`: completely revamped — now includes `Status`, `Score`, `Risk`, `Mode`, `Checks` (detailed verification checks), `Address` (parsed email components), `DidYouMean`, `MxRecords`
- New models: `VerificationChecks`, `VerificationAddress`, `MxRecordInfo`

### API Keys
- `ApiKeyResponse`: added `LastUsedAt` field

## Breaking Changes
- `CancelEmailAsync` return type changed from `Task<bool>` to `Task<CancelEmailResponse>`
- `ListTemplatesAsync` signature changed from `(int page, int limit)` to `(ListTemplatesRequest? request)`
- `ListSubscribersAsync` signature changed to include optional `bool? subscribed` parameter
- `SendEmailRequest`: removed `TrackOpens`, `TrackClicks`, `Metadata` — use `Tracking` and `Meta` instead
- `EmailVerificationResponse`: removed old fields (`RiskScore`, `IsDeliverable`, `IsDisposable`, `IsRoleAccount`, `IsFreeProvider`, `HasMxRecords`, `SmtpProvider`, `VerifiedAt`) — use `Score`, `Risk`, `Checks` instead
- `SubscriberResponse`: removed `Status` — use `Subscribed` bool instead
- `CreateDomainRequest`: removed `FromEmail`
- `DomainResponse`: renamed `TrackOpens` → `TrackLoads`
- `TemplateResponse`: removed `Version` and `Published` — use `PublishedAt` and `Versions` instead
- `DeleteTemplateResponse`: replaced `Object`/`Id`/`Deleted` with `Data`/`Message`

## Test Coverage
- 98 unit tests covering all new and updated endpoints
- New test files: `WebhookTests`, `ContactTests`, `EventTests`, `TemplateTests`, `SuppressionTests`, `SubscriberTests`, `VerificationTests`

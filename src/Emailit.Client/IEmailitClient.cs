using Emailit.Client.Models;
using Emailit.Client.Models.ApiKeys;
using Emailit.Client.Models.Audiences;
using Emailit.Client.Models.Domains;
using Emailit.Client.Models.Emails;
using Emailit.Client.Models.Subscribers;
using Emailit.Client.Models.Suppressions;
using Emailit.Client.Models.Templates;
using Emailit.Client.Models.Verification;
using Emailit.Client.Models.Contacts;
using Emailit.Client.Models.Events;
using Emailit.Client.Models.Webhooks;

namespace Emailit.Client;

/// <summary>
/// Client interface for Emailit API v2.
/// </summary>
public interface IEmailitClient : IDisposable
{
    /// <summary>
    /// Rate limit info from the most recent API response. Thread-safe.
    /// </summary>
    RateLimitInfo? LastRateLimitInfo { get; }

    #region Emails

    /// <summary>
    /// Sends an email.
    /// </summary>
    /// <param name="request">Email request details.</param>
    /// <param name="idempotencyKey">Optional idempotency key to prevent duplicates.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Email response with ID and status.</returns>
    Task<EmailResponse> SendEmailAsync(
        SendEmailRequest request,
        string? idempotencyKey = null,
        CancellationToken ct = default);

    /// <summary>
    /// Gets email details by ID.
    /// </summary>
    /// <param name="emailId">Email ID (prefixed with em_).</param>
    /// <param name="ct">Cancellation token.</param>
    Task<EmailResponse> GetEmailAsync(string emailId, CancellationToken ct = default);

    /// <summary>
    /// Updates a scheduled email's send time.
    /// </summary>
    /// <param name="emailId">Email ID.</param>
    /// <param name="request">Update request with new scheduled time.</param>
    /// <param name="ct">Cancellation token.</param>
    Task<EmailResponse> UpdateScheduledEmailAsync(
        string emailId,
        UpdateScheduledEmailRequest request,
        CancellationToken ct = default);

    /// <summary>
    /// Cancels a scheduled email.
    /// </summary>
    /// <param name="emailId">Email ID.</param>
    /// <param name="ct">Cancellation token.</param>
    Task<CancelEmailResponse> CancelEmailAsync(string emailId, CancellationToken ct = default);

    /// <summary>
    /// Lists emails with cursor-based pagination and optional filters.
    /// </summary>
    /// <param name="request">Optional filters and pagination parameters.</param>
    /// <param name="ct">Cancellation token.</param>
    Task<CursorPaginatedResponse<EmailResponse>> ListEmailsAsync(
        ListEmailsRequest? request = null,
        CancellationToken ct = default);

    /// <summary>
    /// Retries a failed email (creates a new email with a new ID).
    /// </summary>
    /// <param name="emailId">Email ID to retry.</param>
    /// <param name="ct">Cancellation token.</param>
    Task<RetryEmailResponse> RetryEmailAsync(string emailId, CancellationToken ct = default);

    /// <summary>
    /// Resends a failed email. Use RetryEmailAsync instead.
    /// </summary>
    [Obsolete("Use RetryEmailAsync instead. This method will be removed in a future version.")]
    Task<EmailResponse> ResendEmailAsync(string emailId, CancellationToken ct = default);

    /// <summary>
    /// Gets email metadata without body content.
    /// </summary>
    /// <param name="emailId">Email ID (prefixed with em_).</param>
    /// <param name="ct">Cancellation token.</param>
    Task<EmailMetaResponse> GetEmailMetaAsync(string emailId, CancellationToken ct = default);

    /// <summary>
    /// Gets the parsed text and HTML body of an email.
    /// </summary>
    /// <param name="emailId">Email ID (prefixed with em_).</param>
    /// <param name="ct">Cancellation token.</param>
    Task<EmailBodyResponse> GetEmailBodyAsync(string emailId, CancellationToken ct = default);

    /// <summary>
    /// Gets the full raw MIME message of an email.
    /// </summary>
    /// <param name="emailId">Email ID (prefixed with em_).</param>
    /// <param name="ct">Cancellation token.</param>
    Task<EmailRawResponse> GetEmailRawAsync(string emailId, CancellationToken ct = default);

    /// <summary>
    /// Gets email attachments with base64-encoded content.
    /// </summary>
    /// <param name="emailId">Email ID (prefixed with em_).</param>
    /// <param name="ct">Cancellation token.</param>
    Task<EmailAttachmentsResponse> GetEmailAttachmentsAsync(string emailId, CancellationToken ct = default);

    #endregion

    #region Domains

    /// <summary>
    /// Creates a new sending domain.
    /// </summary>
    Task<DomainResponse> CreateDomainAsync(CreateDomainRequest request, CancellationToken ct = default);

    /// <summary>
    /// Gets domain details by ID.
    /// </summary>
    Task<DomainResponse> GetDomainAsync(string domainId, CancellationToken ct = default);

    /// <summary>
    /// Lists all domains with pagination.
    /// </summary>
    Task<PaginatedResponse<DomainResponse>> ListDomainsAsync(int page = 1, int limit = 100, CancellationToken ct = default);

    /// <summary>
    /// Updates a domain's settings.
    /// </summary>
    Task<DomainResponse> UpdateDomainAsync(string domainId, UpdateDomainRequest request, CancellationToken ct = default);

    /// <summary>
    /// Verifies a domain's DNS records.
    /// </summary>
    Task<DomainResponse> VerifyDomainAsync(string domainId, CancellationToken ct = default);

    /// <summary>
    /// Deletes a domain.
    /// </summary>
    Task<DeleteDomainResponse> DeleteDomainAsync(string domainId, CancellationToken ct = default);

    #endregion

    #region API Keys

    /// <summary>
    /// Creates a new API key.
    /// </summary>
    Task<ApiKeyResponse> CreateApiKeyAsync(CreateApiKeyRequest request, CancellationToken ct = default);

    /// <summary>
    /// Gets API key details by ID.
    /// </summary>
    Task<ApiKeyResponse> GetApiKeyAsync(string apiKeyId, CancellationToken ct = default);

    /// <summary>
    /// Lists all API keys with pagination.
    /// </summary>
    Task<PaginatedResponse<ApiKeyResponse>> ListApiKeysAsync(int page = 1, int limit = 100, CancellationToken ct = default);

    /// <summary>
    /// Updates an API key's name.
    /// </summary>
    Task<ApiKeyResponse> UpdateApiKeyAsync(string apiKeyId, UpdateApiKeyRequest request, CancellationToken ct = default);

    /// <summary>
    /// Deletes an API key.
    /// </summary>
    Task<DeleteApiKeyResponse> DeleteApiKeyAsync(string apiKeyId, CancellationToken ct = default);

    #endregion

    #region Audiences

    /// <summary>
    /// Creates a new audience.
    /// </summary>
    Task<AudienceResponse> CreateAudienceAsync(CreateAudienceRequest request, CancellationToken ct = default);

    /// <summary>
    /// Gets audience details by ID.
    /// </summary>
    Task<AudienceResponse> GetAudienceAsync(string audienceId, CancellationToken ct = default);

    /// <summary>
    /// Lists all audiences with pagination.
    /// </summary>
    Task<PaginatedResponse<AudienceResponse>> ListAudiencesAsync(int page = 1, int limit = 100, CancellationToken ct = default);

    /// <summary>
    /// Updates an audience's name.
    /// </summary>
    Task<AudienceResponse> UpdateAudienceAsync(string audienceId, UpdateAudienceRequest request, CancellationToken ct = default);

    /// <summary>
    /// Deletes an audience.
    /// </summary>
    Task<DeleteAudienceResponse> DeleteAudienceAsync(string audienceId, CancellationToken ct = default);

    #endregion

    #region Subscribers

    /// <summary>
    /// Adds a subscriber to an audience.
    /// </summary>
    Task<SubscriberResponse> AddSubscriberAsync(string audienceId, AddSubscriberRequest request, CancellationToken ct = default);

    /// <summary>
    /// Gets subscriber details by ID.
    /// </summary>
    Task<SubscriberResponse> GetSubscriberAsync(string audienceId, string subscriberId, CancellationToken ct = default);

    /// <summary>
    /// Lists all subscribers in an audience with pagination and optional filters.
    /// </summary>
    /// <param name="audienceId">Audience ID.</param>
    /// <param name="page">Page number.</param>
    /// <param name="limit">Items per page.</param>
    /// <param name="subscribed">Filter by subscription status.</param>
    /// <param name="ct">Cancellation token.</param>
    Task<PaginatedResponse<SubscriberResponse>> ListSubscribersAsync(string audienceId, int page = 1, int limit = 100, bool? subscribed = null, CancellationToken ct = default);

    /// <summary>
    /// Updates a subscriber's details.
    /// </summary>
    Task<SubscriberResponse> UpdateSubscriberAsync(string audienceId, string subscriberId, UpdateSubscriberRequest request, CancellationToken ct = default);

    /// <summary>
    /// Deletes a subscriber from an audience.
    /// </summary>
    Task<DeleteSubscriberResponse> DeleteSubscriberAsync(string audienceId, string subscriberId, CancellationToken ct = default);

    #endregion

    #region Templates

    /// <summary>
    /// Creates a new template.
    /// </summary>
    Task<TemplateResponse> CreateTemplateAsync(CreateTemplateRequest request, CancellationToken ct = default);

    /// <summary>
    /// Gets template details by ID.
    /// </summary>
    Task<TemplateResponse> GetTemplateAsync(string templateId, CancellationToken ct = default);

    /// <summary>
    /// Lists all templates with pagination and optional filters.
    /// </summary>
    /// <param name="request">Optional filters and pagination parameters.</param>
    /// <param name="ct">Cancellation token.</param>
    Task<TemplatePaginatedResponse> ListTemplatesAsync(ListTemplatesRequest? request = null, CancellationToken ct = default);

    /// <summary>
    /// Updates a template.
    /// </summary>
    Task<TemplateResponse> UpdateTemplateAsync(string templateId, UpdateTemplateRequest request, CancellationToken ct = default);

    /// <summary>
    /// Publishes a template.
    /// </summary>
    Task<TemplateResponse> PublishTemplateAsync(string templateId, CancellationToken ct = default);

    /// <summary>
    /// Deletes a template.
    /// </summary>
    Task<DeleteTemplateResponse> DeleteTemplateAsync(string templateId, CancellationToken ct = default);

    #endregion

    #region Suppressions

    /// <summary>
    /// Creates a suppression entry.
    /// </summary>
    Task<SuppressionResponse> CreateSuppressionAsync(CreateSuppressionRequest request, CancellationToken ct = default);

    /// <summary>
    /// Gets suppression details by ID.
    /// </summary>
    Task<SuppressionResponse> GetSuppressionAsync(string suppressionId, CancellationToken ct = default);

    /// <summary>
    /// Lists all suppressions with pagination.
    /// </summary>
    Task<PaginatedResponse<SuppressionResponse>> ListSuppressionsAsync(int page = 1, int limit = 100, CancellationToken ct = default);

    /// <summary>
    /// Updates a suppression entry.
    /// </summary>
    Task<SuppressionResponse> UpdateSuppressionAsync(string suppressionId, UpdateSuppressionRequest request, CancellationToken ct = default);

    /// <summary>
    /// Deletes a suppression entry.
    /// </summary>
    Task<DeleteSuppressionResponse> DeleteSuppressionAsync(string suppressionId, CancellationToken ct = default);

    #endregion

    #region Email Verification

    /// <summary>
    /// Verifies a single email address.
    /// </summary>
    Task<EmailVerificationResponse> VerifyEmailAsync(VerifyEmailRequest request, CancellationToken ct = default);

    /// <summary>
    /// Creates a bulk verification list.
    /// </summary>
    Task<VerificationListResponse> CreateVerificationListAsync(CreateVerificationListRequest request, CancellationToken ct = default);

    /// <summary>
    /// Gets verification list details by ID.
    /// </summary>
    Task<VerificationListResponse> GetVerificationListAsync(string listId, CancellationToken ct = default);

    /// <summary>
    /// Lists all verification lists with pagination.
    /// </summary>
    Task<PaginatedResponse<VerificationListResponse>> ListVerificationListsAsync(int page = 1, int limit = 100, CancellationToken ct = default);

    /// <summary>
    /// Gets verification results for a list.
    /// </summary>
    Task<VerificationListResultsResponse> GetVerificationResultsAsync(string listId, int page = 1, int limit = 100, CancellationToken ct = default);

    /// <summary>
    /// Exports verification results to XLSX (returns download URL).
    /// </summary>
    Task<string> ExportVerificationResultsAsync(string listId, CancellationToken ct = default);

    #endregion

    #region Events

    /// <summary>
    /// Lists events with optional filters and pagination.
    /// </summary>
    Task<PaginatedResponse<EventResponse>> ListEventsAsync(ListEventsRequest? request = null, CancellationToken ct = default);

    /// <summary>
    /// Gets event details by ID.
    /// </summary>
    Task<EventResponse> GetEventAsync(string eventId, CancellationToken ct = default);

    #endregion

    #region Contacts

    /// <summary>
    /// Creates a new contact.
    /// </summary>
    Task<ContactResponse> CreateContactAsync(CreateContactRequest request, CancellationToken ct = default);

    /// <summary>
    /// Gets contact details by ID or email address.
    /// </summary>
    Task<ContactResponse> GetContactAsync(string contactIdOrEmail, CancellationToken ct = default);

    /// <summary>
    /// Lists all contacts with pagination.
    /// </summary>
    Task<PaginatedResponse<ContactResponse>> ListContactsAsync(int page = 1, int limit = 100, CancellationToken ct = default);

    /// <summary>
    /// Updates a contact by ID or email address.
    /// </summary>
    Task<ContactResponse> UpdateContactAsync(string contactIdOrEmail, UpdateContactRequest request, CancellationToken ct = default);

    /// <summary>
    /// Deletes a contact.
    /// </summary>
    Task<DeleteContactResponse> DeleteContactAsync(string contactId, CancellationToken ct = default);

    #endregion

    #region Webhooks

    /// <summary>
    /// Creates a new webhook.
    /// </summary>
    Task<WebhookResponse> CreateWebhookAsync(CreateWebhookRequest request, CancellationToken ct = default);

    /// <summary>
    /// Gets webhook details by ID.
    /// </summary>
    Task<WebhookResponse> GetWebhookAsync(string webhookId, CancellationToken ct = default);

    /// <summary>
    /// Lists all webhooks with pagination.
    /// </summary>
    Task<PaginatedResponse<WebhookResponse>> ListWebhooksAsync(int page = 1, int limit = 100, CancellationToken ct = default);

    /// <summary>
    /// Updates a webhook.
    /// </summary>
    Task<WebhookResponse> UpdateWebhookAsync(string webhookId, UpdateWebhookRequest request, CancellationToken ct = default);

    /// <summary>
    /// Deletes a webhook.
    /// </summary>
    Task<DeleteWebhookResponse> DeleteWebhookAsync(string webhookId, CancellationToken ct = default);

    #endregion

    #region Connection Test

    /// <summary>
    /// Tests the API connection and returns rate limit information.
    /// </summary>
    /// <returns>Rate limit info if successful, null otherwise.</returns>
    Task<RateLimitInfo?> TestConnectionAsync(CancellationToken ct = default);

    #endregion
}

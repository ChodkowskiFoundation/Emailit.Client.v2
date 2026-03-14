using System.Text.Json;
using System.Text.Json.Serialization;

using Emailit.Client.Exceptions;
using Emailit.Client.Models;
using Emailit.Client.Models.ApiKeys;
using Emailit.Client.Models.Audiences;
using Emailit.Client.Models.Domains;
using Emailit.Client.Models.Emails;
using Emailit.Client.Models.Subscribers;
using Emailit.Client.Models.Suppressions;
using Emailit.Client.Models.Templates;
using Emailit.Client.Models.Contacts;
using Emailit.Client.Models.Events;
using Emailit.Client.Models.Verification;
using Emailit.Client.Models.Webhooks;

using Flurl;
using Flurl.Http;

using Microsoft.Extensions.Options;

namespace Emailit.Client;

/// <summary>
/// Emailit API v2 client implementation using Flurl.
/// </summary>
public sealed class EmailitClient : IEmailitClient
{
    private readonly EmailitClientOptions _options;
    private readonly FlurlClient _client;
    private bool _disposed;
    private volatile RateLimitInfo? _lastRateLimitInfo;

    /// <inheritdoc />
    public RateLimitInfo? LastRateLimitInfo => _lastRateLimitInfo;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        Converters =
        {
            new FlexibleBooleanConverter(),
            new FlexibleNullableBooleanConverter(),
            new FlexibleDateTimeConverter(),
            new FlexibleNullableDateTimeConverter(),
            new FlexibleStringListConverter()
        }
    };

    /// <summary>
    /// Creates a new Emailit client with the specified options.
    /// </summary>
    /// <param name="options">Client configuration options.</param>
    public EmailitClient(EmailitClientOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _options.Validate();

        _client = new FlurlClient(_options.BaseUrl)
            .WithTimeout(TimeSpan.FromSeconds(_options.TimeoutSeconds))
            .WithHeader("Authorization", $"Bearer {_options.ApiKey}")
            .WithHeader("Content-Type", "application/json")
            .WithHeader("Accept", "application/json");
    }

    /// <summary>
    /// Creates a new Emailit client using IOptions pattern (for DI).
    /// </summary>
    public EmailitClient(IOptions<EmailitClientOptions> options) : this(options.Value)
    {
    }

    #region Emails

    public Task<EmailResponse> SendEmailAsync(
        SendEmailRequest request,
        string? idempotencyKey = null,
        CancellationToken ct = default)
    {
        var flurlRequest = _client.Request("/v2/emails");

        if (!string.IsNullOrEmpty(idempotencyKey))
            flurlRequest.WithHeader("Idempotency-Key", idempotencyKey);

        return ExecuteAsync<EmailResponse>(
            () => flurlRequest.PostJsonAsync(request, cancellationToken: ct),
            ct);
    }

    public Task<EmailResponse> GetEmailAsync(string emailId, CancellationToken ct = default) =>
        ExecuteAsync<EmailResponse>(
            () => _client.Request("/v2/emails", emailId).GetAsync(cancellationToken: ct),
            ct);

    public Task<EmailResponse> UpdateScheduledEmailAsync(
        string emailId,
        UpdateScheduledEmailRequest request,
        CancellationToken ct = default) =>
        ExecuteAsync<EmailResponse>(
            () => _client.Request("/v2/emails", emailId).PostJsonAsync(request, cancellationToken: ct),
            ct);

    public Task<CancelEmailResponse> CancelEmailAsync(string emailId, CancellationToken ct = default) =>
        ExecuteAsync<CancelEmailResponse>(
            () => _client.Request("/v2/emails", emailId, "cancel").PostAsync(cancellationToken: ct),
            ct);

    public Task<CursorPaginatedResponse<EmailResponse>> ListEmailsAsync(
        ListEmailsRequest? request = null,
        CancellationToken ct = default)
    {
        var req = _client.Request("/v2/emails");

        if (request != null)
        {
            req.SetQueryParam("limit", request.Limit);

            if (!string.IsNullOrEmpty(request.After))
                req.SetQueryParam("after", request.After);
            if (!string.IsNullOrEmpty(request.Status))
                req.SetQueryParam("status", request.Status);
            if (!string.IsNullOrEmpty(request.Tag))
                req.SetQueryParam("tag", request.Tag);
            if (!string.IsNullOrEmpty(request.From))
                req.SetQueryParam("from", request.From);
            if (!string.IsNullOrEmpty(request.To))
                req.SetQueryParam("to", request.To);
            if (!string.IsNullOrEmpty(request.Subject))
                req.SetQueryParam("subject", request.Subject);
            if (!string.IsNullOrEmpty(request.CreatedAfter))
                req.SetQueryParam("created_after", request.CreatedAfter);
            if (!string.IsNullOrEmpty(request.CreatedBefore))
                req.SetQueryParam("created_before", request.CreatedBefore);
            if (!string.IsNullOrEmpty(request.Type))
                req.SetQueryParam("type", request.Type);
        }

        return ExecuteAsync<CursorPaginatedResponse<EmailResponse>>(
            () => req.GetAsync(cancellationToken: ct), ct);
    }

    public Task<RetryEmailResponse> RetryEmailAsync(string emailId, CancellationToken ct = default) =>
        ExecuteAsync<RetryEmailResponse>(
            () => _client.Request("/v2/emails", emailId, "retry").PostAsync(cancellationToken: ct),
            ct);

    [Obsolete("Use RetryEmailAsync instead. This method will be removed in a future version.")]
    public async Task<EmailResponse> ResendEmailAsync(string emailId, CancellationToken ct = default)
    {
        try
        {
            return await ExecuteAsync<EmailResponse>(
                () => _client.Request("/v2/emails", emailId, "resend").PostAsync(cancellationToken: ct),
                ct);
        }
        catch (EmailitNotFoundException)
        {
            var retriedEmail = await RetryEmailAsync(emailId, ct);
            return new EmailResponse
            {
                Id = retriedEmail.Id,
                Status = retriedEmail.Status,
                Message = retriedEmail.Message,
                CreatedAt = DateTime.UtcNow
            };
        }
    }

    public Task<EmailMetaResponse> GetEmailMetaAsync(string emailId, CancellationToken ct = default) =>
        ExecuteAsync<EmailMetaResponse>(
            () => _client.Request("/v2/emails", emailId, "meta").GetAsync(cancellationToken: ct),
            ct);

    public Task<EmailBodyResponse> GetEmailBodyAsync(string emailId, CancellationToken ct = default) =>
        ExecuteAsync<EmailBodyResponse>(
            () => _client.Request("/v2/emails", emailId, "body").GetAsync(cancellationToken: ct),
            ct);

    public Task<EmailRawResponse> GetEmailRawAsync(string emailId, CancellationToken ct = default) =>
        ExecuteAsync<EmailRawResponse>(
            () => _client.Request("/v2/emails", emailId, "raw").GetAsync(cancellationToken: ct),
            ct);

    public Task<EmailAttachmentsResponse> GetEmailAttachmentsAsync(string emailId, CancellationToken ct = default) =>
        ExecuteAsync<EmailAttachmentsResponse>(
            () => _client.Request("/v2/emails", emailId, "attachments").GetAsync(cancellationToken: ct),
            ct);

    #endregion

    #region Domains

    public Task<DomainResponse> CreateDomainAsync(CreateDomainRequest request, CancellationToken ct = default) =>
        ExecuteAsync<DomainResponse>(
            () => _client.Request("/v2/domains").PostJsonAsync(request, cancellationToken: ct),
            ct);

    public Task<DomainResponse> GetDomainAsync(string domainId, CancellationToken ct = default) =>
        ExecuteAsync<DomainResponse>(
            () => _client.Request("/v2/domains", domainId).GetAsync(cancellationToken: ct),
            ct);

    public Task<PaginatedResponse<DomainResponse>> ListDomainsAsync(int page = 1, int limit = 100, CancellationToken ct = default) =>
        ExecuteAsync<PaginatedResponse<DomainResponse>>(
            () => _client.Request("/v2/domains")
                .SetQueryParams(new { page, limit })
                .GetAsync(cancellationToken: ct),
            ct);

    public Task<DomainResponse> UpdateDomainAsync(string domainId, UpdateDomainRequest request, CancellationToken ct = default) =>
        ExecuteAsync<DomainResponse>(
            () => _client.Request("/v2/domains", domainId).PostJsonAsync(request, cancellationToken: ct),
            ct);

    public Task<DomainResponse> VerifyDomainAsync(string domainId, CancellationToken ct = default) =>
        ExecuteAsync<DomainResponse>(
            () => _client.Request("/v2/domains", domainId, "verify").PostAsync(cancellationToken: ct),
            ct);

    public Task<DeleteDomainResponse> DeleteDomainAsync(string domainId, CancellationToken ct = default) =>
        ExecuteAsync<DeleteDomainResponse>(
            () => _client.Request("/v2/domains", domainId).DeleteAsync(cancellationToken: ct),
            ct);

    #endregion

    #region API Keys

    public Task<ApiKeyResponse> CreateApiKeyAsync(CreateApiKeyRequest request, CancellationToken ct = default) =>
        ExecuteAsync<ApiKeyResponse>(
            () => _client.Request("/v2/api-keys").PostJsonAsync(request, cancellationToken: ct),
            ct);

    public Task<ApiKeyResponse> GetApiKeyAsync(string apiKeyId, CancellationToken ct = default) =>
        ExecuteAsync<ApiKeyResponse>(
            () => _client.Request("/v2/api-keys", apiKeyId).GetAsync(cancellationToken: ct),
            ct);

    public Task<PaginatedResponse<ApiKeyResponse>> ListApiKeysAsync(int page = 1, int limit = 100, CancellationToken ct = default) =>
        ExecuteAsync<PaginatedResponse<ApiKeyResponse>>(
            () => _client.Request("/v2/api-keys")
                .SetQueryParams(new { page, limit })
                .GetAsync(cancellationToken: ct),
            ct);

    public Task<ApiKeyResponse> UpdateApiKeyAsync(string apiKeyId, UpdateApiKeyRequest request, CancellationToken ct = default) =>
        ExecuteAsync<ApiKeyResponse>(
            () => _client.Request("/v2/api-keys", apiKeyId).PostJsonAsync(request, cancellationToken: ct),
            ct);

    public Task<DeleteApiKeyResponse> DeleteApiKeyAsync(string apiKeyId, CancellationToken ct = default) =>
        ExecuteAsync<DeleteApiKeyResponse>(
            () => _client.Request("/v2/api-keys", apiKeyId).DeleteAsync(cancellationToken: ct),
            ct);

    #endregion

    #region Audiences

    public Task<AudienceResponse> CreateAudienceAsync(CreateAudienceRequest request, CancellationToken ct = default) =>
        ExecuteAsync<AudienceResponse>(
            () => _client.Request("/v2/audiences").PostJsonAsync(request, cancellationToken: ct),
            ct);

    public Task<AudienceResponse> GetAudienceAsync(string audienceId, CancellationToken ct = default) =>
        ExecuteAsync<AudienceResponse>(
            () => _client.Request("/v2/audiences", audienceId).GetAsync(cancellationToken: ct),
            ct);

    public Task<PaginatedResponse<AudienceResponse>> ListAudiencesAsync(int page = 1, int limit = 100, CancellationToken ct = default) =>
        ExecuteAsync<PaginatedResponse<AudienceResponse>>(
            () => _client.Request("/v2/audiences")
                .SetQueryParams(new { page, limit })
                .GetAsync(cancellationToken: ct),
            ct);

    public Task<AudienceResponse> UpdateAudienceAsync(string audienceId, UpdateAudienceRequest request, CancellationToken ct = default) =>
        ExecuteAsync<AudienceResponse>(
            () => _client.Request("/v2/audiences", audienceId).PostJsonAsync(request, cancellationToken: ct),
            ct);

    public Task<DeleteAudienceResponse> DeleteAudienceAsync(string audienceId, CancellationToken ct = default) =>
        ExecuteAsync<DeleteAudienceResponse>(
            () => _client.Request("/v2/audiences", audienceId).DeleteAsync(cancellationToken: ct),
            ct);

    #endregion

    #region Subscribers

    public Task<SubscriberResponse> AddSubscriberAsync(string audienceId, AddSubscriberRequest request, CancellationToken ct = default) =>
        ExecuteAsync<SubscriberResponse>(
            () => _client.Request("/v2/audiences", audienceId, "subscribers").PostJsonAsync(request, cancellationToken: ct),
            ct);

    public Task<SubscriberResponse> GetSubscriberAsync(string audienceId, string subscriberId, CancellationToken ct = default) =>
        ExecuteAsync<SubscriberResponse>(
            () => _client.Request("/v2/audiences", audienceId, "subscribers", subscriberId).GetAsync(cancellationToken: ct),
            ct);

    public Task<PaginatedResponse<SubscriberResponse>> ListSubscribersAsync(string audienceId, int page = 1, int limit = 100, bool? subscribed = null, CancellationToken ct = default)
    {
        var req = _client.Request("/v2/audiences", audienceId, "subscribers")
            .SetQueryParams(new { page, limit });

        if (subscribed.HasValue)
            req.SetQueryParam("subscribed", subscribed.Value.ToString().ToLowerInvariant());

        return ExecuteAsync<PaginatedResponse<SubscriberResponse>>(
            () => req.GetAsync(cancellationToken: ct), ct);
    }

    public Task<SubscriberResponse> UpdateSubscriberAsync(string audienceId, string subscriberId, UpdateSubscriberRequest request, CancellationToken ct = default) =>
        ExecuteAsync<SubscriberResponse>(
            () => _client.Request("/v2/audiences", audienceId, "subscribers", subscriberId).PostJsonAsync(request, cancellationToken: ct),
            ct);

    public Task<DeleteSubscriberResponse> DeleteSubscriberAsync(string audienceId, string subscriberId, CancellationToken ct = default) =>
        ExecuteAsync<DeleteSubscriberResponse>(
            () => _client.Request("/v2/audiences", audienceId, "subscribers", subscriberId).DeleteAsync(cancellationToken: ct),
            ct);

    #endregion

    #region Templates

    public async Task<TemplateResponse> CreateTemplateAsync(CreateTemplateRequest request, CancellationToken ct = default)
    {
        var wrapper = await ExecuteAsync<TemplateDataResponse>(
            () => _client.Request("/v2/templates").PostJsonAsync(request, cancellationToken: ct),
            ct);
        return wrapper.Data ?? throw CreateUnexpectedResponseException(
            BuildManualContext(HttpMethod.Post, "/v2/templates"),
            "Template response did not contain a data payload.");
    }

    public async Task<TemplateResponse> GetTemplateAsync(string templateId, CancellationToken ct = default)
    {
        var wrapper = await ExecuteAsync<TemplateDataResponse>(
            () => _client.Request("/v2/templates", templateId).GetAsync(cancellationToken: ct),
            ct);
        return wrapper.Data ?? throw CreateUnexpectedResponseException(
            BuildManualContext(HttpMethod.Get, $"/v2/templates/{templateId}"),
            "Template response did not contain a data payload.");
    }

    public Task<TemplatePaginatedResponse> ListTemplatesAsync(ListTemplatesRequest? request = null, CancellationToken ct = default)
    {
        var req = _client.Request("/v2/templates");

        var page = request?.Page ?? 1;
        var perPage = request?.PerPage ?? 25;
        req.SetQueryParams(new { page, per_page = perPage });

        if (request != null)
        {
            if (!string.IsNullOrEmpty(request.FilterName))
                req.SetQueryParam("filter[name]", request.FilterName);
            if (!string.IsNullOrEmpty(request.FilterAlias))
                req.SetQueryParam("filter[alias]", request.FilterAlias);
            if (!string.IsNullOrEmpty(request.FilterEditor))
                req.SetQueryParam("filter[editor]", request.FilterEditor);
            if (!string.IsNullOrEmpty(request.Sort))
                req.SetQueryParam("sort", request.Sort);
            if (!string.IsNullOrEmpty(request.Order))
                req.SetQueryParam("order", request.Order);
        }

        return ExecuteAsync<TemplatePaginatedResponse>(
            () => req.GetAsync(cancellationToken: ct), ct);
    }

    public async Task<TemplateResponse> UpdateTemplateAsync(string templateId, UpdateTemplateRequest request, CancellationToken ct = default)
    {
        var wrapper = await ExecuteAsync<TemplateDataResponse>(
            () => _client.Request("/v2/templates", templateId).PostJsonAsync(request, cancellationToken: ct),
            ct);
        return wrapper.Data ?? throw CreateUnexpectedResponseException(
            BuildManualContext(HttpMethod.Post, $"/v2/templates/{templateId}"),
            "Template response did not contain a data payload.");
    }

    public async Task<TemplateResponse> PublishTemplateAsync(string templateId, CancellationToken ct = default)
    {
        var wrapper = await ExecuteAsync<TemplateDataResponse>(
            () => _client.Request("/v2/templates", templateId, "publish").PostAsync(cancellationToken: ct),
            ct);
        return wrapper.Data ?? throw CreateUnexpectedResponseException(
            BuildManualContext(HttpMethod.Post, $"/v2/templates/{templateId}/publish"),
            "Template response did not contain a data payload.");
    }

    public Task<DeleteTemplateResponse> DeleteTemplateAsync(string templateId, CancellationToken ct = default) =>
        ExecuteAsync<DeleteTemplateResponse>(
            () => _client.Request("/v2/templates", templateId).DeleteAsync(cancellationToken: ct),
            ct);

    #endregion

    #region Suppressions

    public Task<SuppressionResponse> CreateSuppressionAsync(CreateSuppressionRequest request, CancellationToken ct = default) =>
        ExecuteAsync<SuppressionResponse>(
            () => _client.Request("/v2/suppressions").PostJsonAsync(request, cancellationToken: ct),
            ct);

    public Task<SuppressionResponse> GetSuppressionAsync(string suppressionId, CancellationToken ct = default) =>
        ExecuteAsync<SuppressionResponse>(
            () => _client.Request("/v2/suppressions", suppressionId).GetAsync(cancellationToken: ct),
            ct);

    public Task<PaginatedResponse<SuppressionResponse>> ListSuppressionsAsync(int page = 1, int limit = 100, CancellationToken ct = default) =>
        ExecuteAsync<PaginatedResponse<SuppressionResponse>>(
            () => _client.Request("/v2/suppressions")
                .SetQueryParams(new { page, limit })
                .GetAsync(cancellationToken: ct),
            ct);

    public Task<SuppressionResponse> UpdateSuppressionAsync(string suppressionId, UpdateSuppressionRequest request, CancellationToken ct = default) =>
        ExecuteAsync<SuppressionResponse>(
            () => _client.Request("/v2/suppressions", suppressionId).PostJsonAsync(request, cancellationToken: ct),
            ct);

    public Task<DeleteSuppressionResponse> DeleteSuppressionAsync(string suppressionId, CancellationToken ct = default) =>
        ExecuteAsync<DeleteSuppressionResponse>(
            () => _client.Request("/v2/suppressions", suppressionId).DeleteAsync(cancellationToken: ct),
            ct);

    #endregion

    #region Email Verification

    public Task<EmailVerificationResponse> VerifyEmailAsync(VerifyEmailRequest request, CancellationToken ct = default) =>
        ExecuteAsync<EmailVerificationResponse>(
            () => _client.Request("/v2/email-verifications").PostJsonAsync(request, cancellationToken: ct),
            ct);

    public Task<VerificationListResponse> CreateVerificationListAsync(CreateVerificationListRequest request, CancellationToken ct = default) =>
        ExecuteAsync<VerificationListResponse>(
            () => _client.Request("/v2/email-verification-lists").PostJsonAsync(request, cancellationToken: ct),
            ct);

    public Task<VerificationListResponse> GetVerificationListAsync(string listId, CancellationToken ct = default) =>
        ExecuteAsync<VerificationListResponse>(
            () => _client.Request("/v2/email-verification-lists", listId).GetAsync(cancellationToken: ct),
            ct);

    public Task<PaginatedResponse<VerificationListResponse>> ListVerificationListsAsync(int page = 1, int limit = 100, CancellationToken ct = default) =>
        ExecuteAsync<PaginatedResponse<VerificationListResponse>>(
            () => _client.Request("/v2/email-verification-lists")
                .SetQueryParams(new { page, limit })
                .GetAsync(cancellationToken: ct),
            ct);

    public Task<VerificationListResultsResponse> GetVerificationResultsAsync(string listId, int page = 1, int limit = 100, CancellationToken ct = default) =>
        ExecuteAsync<VerificationListResultsResponse>(
            () => _client.Request("/v2/email-verification-lists", listId, "results")
                .SetQueryParams(new { page, limit })
                .GetAsync(cancellationToken: ct),
            ct);

    public async Task<string> ExportVerificationResultsAsync(string listId, CancellationToken ct = default)
    {
        try
        {
            var response = await _client.Request("/v2/email-verification-lists", listId, "export")
                .GetAsync(cancellationToken: ct);

            _lastRateLimitInfo = ParseRateLimitInfo(response);

            return response.ResponseMessage.RequestMessage?.RequestUri?.ToString()
                ?? throw CreateUnexpectedResponseException(
                    CreateSuccessContext(response, responseBody: null),
                    "Emailit did not return an export URL.");
        }
        catch (FlurlHttpTimeoutException ex)
        {
            RethrowIfCallerCanceled(ex, ct);
            throw CreateTimeoutException(ex);
        }
        catch (FlurlHttpException ex)
        {
            RethrowIfCallerCanceled(ex, ct);
            throw await CreateExceptionAsync(ex);
        }
    }

    #endregion

    #region Events

    public Task<PaginatedResponse<EventResponse>> ListEventsAsync(ListEventsRequest? request = null, CancellationToken ct = default)
    {
        var req = _client.Request("/v2/events");

        if (request != null)
        {
            req.SetQueryParams(new { page = request.Page, limit = request.Limit });

            if (!string.IsNullOrEmpty(request.Type))
                req.SetQueryParam("type", request.Type);
            if (request.IncludeData == true)
                req.SetQueryParam("include_data", "true");
        }

        return ExecuteAsync<PaginatedResponse<EventResponse>>(
            () => req.GetAsync(cancellationToken: ct), ct);
    }

    public Task<EventResponse> GetEventAsync(string eventId, CancellationToken ct = default) =>
        ExecuteAsync<EventResponse>(
            () => _client.Request("/v2/events", eventId).GetAsync(cancellationToken: ct),
            ct);

    #endregion

    #region Contacts

    public Task<ContactResponse> CreateContactAsync(CreateContactRequest request, CancellationToken ct = default) =>
        ExecuteAsync<ContactResponse>(
            () => _client.Request("/v2/contacts").PostJsonAsync(request, cancellationToken: ct),
            ct);

    public Task<ContactResponse> GetContactAsync(string contactIdOrEmail, CancellationToken ct = default) =>
        ExecuteAsync<ContactResponse>(
            () => _client.Request("/v2/contacts", contactIdOrEmail).GetAsync(cancellationToken: ct),
            ct);

    public Task<PaginatedResponse<ContactResponse>> ListContactsAsync(int page = 1, int limit = 100, CancellationToken ct = default) =>
        ExecuteAsync<PaginatedResponse<ContactResponse>>(
            () => _client.Request("/v2/contacts")
                .SetQueryParams(new { page, limit })
                .GetAsync(cancellationToken: ct),
            ct);

    public Task<ContactResponse> UpdateContactAsync(string contactIdOrEmail, UpdateContactRequest request, CancellationToken ct = default) =>
        ExecuteAsync<ContactResponse>(
            () => _client.Request("/v2/contacts", contactIdOrEmail).PostJsonAsync(request, cancellationToken: ct),
            ct);

    public Task<DeleteContactResponse> DeleteContactAsync(string contactId, CancellationToken ct = default) =>
        ExecuteAsync<DeleteContactResponse>(
            () => _client.Request("/v2/contacts", contactId).DeleteAsync(cancellationToken: ct),
            ct);

    #endregion

    #region Webhooks

    public Task<WebhookResponse> CreateWebhookAsync(CreateWebhookRequest request, CancellationToken ct = default) =>
        ExecuteAsync<WebhookResponse>(
            () => _client.Request("/v2/webhooks").PostJsonAsync(request, cancellationToken: ct),
            ct);

    public Task<WebhookResponse> GetWebhookAsync(string webhookId, CancellationToken ct = default) =>
        ExecuteAsync<WebhookResponse>(
            () => _client.Request("/v2/webhooks", webhookId).GetAsync(cancellationToken: ct),
            ct);

    public Task<PaginatedResponse<WebhookResponse>> ListWebhooksAsync(int page = 1, int limit = 100, CancellationToken ct = default) =>
        ExecuteAsync<PaginatedResponse<WebhookResponse>>(
            () => _client.Request("/v2/webhooks")
                .SetQueryParams(new { page, limit })
                .GetAsync(cancellationToken: ct),
            ct);

    public Task<WebhookResponse> UpdateWebhookAsync(string webhookId, UpdateWebhookRequest request, CancellationToken ct = default) =>
        ExecuteAsync<WebhookResponse>(
            () => _client.Request("/v2/webhooks", webhookId).PostJsonAsync(request, cancellationToken: ct),
            ct);

    public Task<DeleteWebhookResponse> DeleteWebhookAsync(string webhookId, CancellationToken ct = default) =>
        ExecuteAsync<DeleteWebhookResponse>(
            () => _client.Request("/v2/webhooks", webhookId).DeleteAsync(cancellationToken: ct),
            ct);

    #endregion

    #region Connection Test

    public async Task<RateLimitInfo?> TestConnectionAsync(CancellationToken ct = default)
    {
        try
        {
            var response = await _client.Request("/v2/domains")
                .GetAsync(cancellationToken: ct);

            var rateLimitInfo = ParseRateLimitInfo(response);
            _lastRateLimitInfo = rateLimitInfo;

            return rateLimitInfo;
        }
        catch (FlurlHttpTimeoutException ex)
        {
            RethrowIfCallerCanceled(ex, ct);
            throw CreateTimeoutException(ex);
        }
        catch (FlurlHttpException ex)
        {
            RethrowIfCallerCanceled(ex, ct);
            throw await CreateExceptionAsync(ex);
        }
    }

    #endregion

    public void Dispose()
    {
        if (_disposed) return;
        _client.Dispose();
        _disposed = true;
    }

    #region Private Helpers

    private async Task<T> ExecuteAsync<T>(Func<Task<IFlurlResponse>> requestFunc, CancellationToken ct)
    {
        try
        {
            var response = await requestFunc();
            return await DeserializeResponseAsync<T>(response, ct);
        }
        catch (FlurlHttpTimeoutException ex)
        {
            RethrowIfCallerCanceled(ex, ct);
            throw CreateTimeoutException(ex);
        }
        catch (FlurlHttpException ex)
        {
            RethrowIfCallerCanceled(ex, ct);
            throw await CreateExceptionAsync(ex);
        }
    }

    private async Task<T> DeserializeResponseAsync<T>(IFlurlResponse response, CancellationToken ct)
    {
        var rateLimitInfo = ParseRateLimitInfo(response);
        _lastRateLimitInfo = rateLimitInfo;

        var body = await response.GetStringAsync();
        var context = CreateSuccessContext(response, body);

        T? result;
        try
        {
            result = JsonSerializer.Deserialize<T>(body, JsonOptions);
        }
        catch (JsonException ex)
        {
            throw new EmailitSerializationException(
                $"Failed to deserialize Emailit response as {typeof(T).Name}.",
                context,
                ex);
        }

        if (result is null)
        {
            throw CreateUnexpectedResponseException(
                context,
                $"Emailit returned an empty response body for {typeof(T).Name}.");
        }

        if (result is EmailResponse emailResponse)
            return (T)(object)(emailResponse with { RateLimitInfo = rateLimitInfo });

        return result;
    }

    private static RateLimitInfo ParseRateLimitInfo(IFlurlResponse response) =>
        RateLimitInfo.FromHeaders(
            response.Headers.ToDictionary(
                h => h.Name,
                h => h.Value,
                StringComparer.OrdinalIgnoreCase));

    private async Task<EmailitException> CreateExceptionAsync(FlurlHttpException ex)
    {
        if (ex.Call?.Response is null)
            return new EmailitTransportException("Failed to reach the Emailit API.", CreateTransportContext(ex), ex);

        var statusCode = ex.StatusCode ?? 0;
        var body = await SafeGetResponseStringAsync(ex);

        ErrorResponse? errorResponse = null;
        try
        {
            errorResponse = JsonSerializer.Deserialize<ErrorResponse>(body, JsonOptions);
        }
        catch
        {
            // Ignore deserialization errors
        }

        var response = ex.Call.Response;
        var rateLimitInfo = ParseRateLimitInfo(response);
        _lastRateLimitInfo = rateLimitInfo;

        var context = CreateErrorContext(response, body, errorResponse, statusCode, rateLimitInfo);
        var message = errorResponse?.GetErrorMessage() ?? ex.Message;

        return statusCode switch
        {
            400 => new EmailitValidationException(message, errorResponse?.GetValidationErrors(), context, ex),
            401 => new EmailitAuthenticationException(message, context, ex),
            403 => new EmailitAuthorizationException(message, context, ex),
            404 => new EmailitNotFoundException(message, context, ex),
            409 => new EmailitConflictException(message, context, ex),
            413 => new EmailitMessageTooLargeException(errorResponse?.Details ?? message, context, ex),
            422 => new EmailitUnprocessableEntityException(message, errorResponse?.GetValidationErrors(), context, ex),
            429 when rateLimitInfo?.IsDailyLimitReached == true => new DailyLimitExceededException(rateLimitInfo, context, ex),
            429 => new RateLimitExceededException(message, rateLimitInfo, context, ex),
            >= 500 => new EmailitServerException(message, context, ex),
            _ => new EmailitApiException(message, context, ex)
        };
    }

    private EmailitTimeoutException CreateTimeoutException(FlurlHttpTimeoutException ex)
    {
        var message = $"The Emailit API request timed out after {_options.TimeoutSeconds} seconds.";
        return new EmailitTimeoutException(message, CreateTransportContext(ex), ex);
    }

    private EmailitExceptionContext CreateSuccessContext(IFlurlResponse response, string? responseBody)
    {
        var headers = response.Headers.ToDictionary(
            h => h.Name,
            h => h.Value,
            StringComparer.OrdinalIgnoreCase);

        var requestMessage = response.ResponseMessage.RequestMessage;
        return BuildContext(
            statusCode: (int)response.StatusCode,
            errorCode: null,
            errorType: null,
            requestMethod: requestMessage?.Method.Method,
            requestUri: requestMessage?.RequestUri,
            requestId: FindCorrelationId(headers),
            rateLimitInfo: ParseRateLimitInfo(response),
            isTransient: false,
            responseBody: responseBody,
            responseHeaders: headers);
    }

    private EmailitExceptionContext CreateErrorContext(
        IFlurlResponse response,
        string? responseBody,
        ErrorResponse? errorResponse,
        int statusCode,
        RateLimitInfo? rateLimitInfo)
    {
        var headers = response.Headers.ToDictionary(
            h => h.Name,
            h => h.Value,
            StringComparer.OrdinalIgnoreCase);

        var requestMessage = response.ResponseMessage.RequestMessage;
        return BuildContext(
            statusCode: statusCode,
            errorCode: errorResponse?.Code,
            errorType: errorResponse?.Type ?? errorResponse?.Error,
            requestMethod: requestMessage?.Method.Method,
            requestUri: requestMessage?.RequestUri,
            requestId: FindCorrelationId(headers),
            rateLimitInfo: rateLimitInfo,
            isTransient: IsTransientStatusCode(statusCode),
            responseBody: responseBody,
            responseHeaders: headers);
    }

    private EmailitExceptionContext CreateTransportContext(FlurlHttpException ex)
    {
        Uri? requestUri = null;
        if (Uri.TryCreate(ex.Call?.Request?.Url?.ToString(), UriKind.Absolute, out var parsedUri))
            requestUri = parsedUri;

        return BuildContext(
            statusCode: null,
            errorCode: null,
            errorType: null,
            requestMethod: ex.Call?.Request?.Verb?.ToString(),
            requestUri: requestUri,
            requestId: null,
            rateLimitInfo: null,
            isTransient: true,
            responseBody: null,
            responseHeaders: null);
    }

    private EmailitExceptionContext BuildManualContext(HttpMethod method, string relativePath) =>
        BuildContext(
            statusCode: null,
            errorCode: null,
            errorType: null,
            requestMethod: method.Method,
            requestUri: CreateAbsoluteUri(relativePath),
            requestId: null,
            rateLimitInfo: null,
            isTransient: false,
            responseBody: null,
            responseHeaders: null);

    private EmailitExceptionContext BuildContext(
        int? statusCode,
        string? errorCode,
        string? errorType,
        string? requestMethod,
        Uri? requestUri,
        string? requestId,
        RateLimitInfo? rateLimitInfo,
        bool isTransient,
        string? responseBody,
        IReadOnlyDictionary<string, string>? responseHeaders)
    {
        var sanitizedUri = _options.ExceptionDetailMode >= EmailitExceptionDetailMode.Safe
            ? SanitizeRequestUri(requestUri)
            : null;

        return new EmailitExceptionContext
        {
            StatusCode = statusCode,
            ErrorCode = errorCode,
            ErrorType = errorType,
            RequestMethod = _options.ExceptionDetailMode >= EmailitExceptionDetailMode.Safe ? requestMethod : null,
            RequestUri = sanitizedUri,
            RequestPath = sanitizedUri?.AbsolutePath,
            RequestId = _options.ExceptionDetailMode >= EmailitExceptionDetailMode.Safe ? requestId : null,
            RateLimitInfo = rateLimitInfo,
            IsTransient = isTransient,
            ResponseBody = _options.ExceptionDetailMode == EmailitExceptionDetailMode.Diagnostic
                ? Truncate(responseBody)
                : null,
            ResponseHeaders = _options.ExceptionDetailMode == EmailitExceptionDetailMode.Diagnostic
                ? responseHeaders
                : null
        };
    }

    private static bool IsTransientStatusCode(int statusCode) =>
        statusCode is 408 or 425 or 429 || statusCode >= 500;

    private static string? FindCorrelationId(IReadOnlyDictionary<string, string> headers)
    {
        foreach (var headerName in new[] { "x-request-id", "request-id", "x-correlation-id", "trace-id" })
        {
            if (headers.TryGetValue(headerName, out var value) && !string.IsNullOrWhiteSpace(value))
                return value;

            foreach (var pair in headers)
            {
                if (string.Equals(pair.Key, headerName, StringComparison.OrdinalIgnoreCase) &&
                    !string.IsNullOrWhiteSpace(pair.Value))
                {
                    return pair.Value;
                }
            }
        }

        return null;
    }

    private Uri? SanitizeRequestUri(Uri? requestUri)
    {
        if (requestUri is null)
            return null;

        var sanitized = requestUri.GetLeftPart(UriPartial.Path);
        return Uri.TryCreate(sanitized, UriKind.Absolute, out var parsed) ? parsed : null;
    }

    private string? Truncate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return value;

        return value.Length <= _options.MaxDiagnosticBodyLength
            ? value
            : value[.._options.MaxDiagnosticBodyLength];
    }

    private Uri? CreateAbsoluteUri(string relativePath)
    {
        if (!Uri.TryCreate(_options.BaseUrl, UriKind.Absolute, out var baseUri))
            return null;

        return Uri.TryCreate(baseUri, relativePath, out var absoluteUri) ? absoluteUri : null;
    }

    private EmailitUnexpectedResponseException CreateUnexpectedResponseException(
        EmailitExceptionContext context,
        string message,
        Exception? innerException = null) =>
        new(message, context, innerException);

    private static void RethrowIfCallerCanceled(FlurlHttpException ex, CancellationToken ct)
    {
        if (ct.IsCancellationRequested && ex.InnerException is OperationCanceledException operationCanceledException)
            throw new OperationCanceledException("The operation was canceled.", operationCanceledException, ct);
    }

    private static async Task<string> SafeGetResponseStringAsync(FlurlHttpException ex)
    {
        try
        {
            return await ex.GetResponseStringAsync() ?? string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }

    #endregion
}

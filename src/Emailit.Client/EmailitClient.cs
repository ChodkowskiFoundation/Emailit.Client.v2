using System.Text.Json;

using Emailit.Client.Exceptions;
using Emailit.Client.Models;
using Emailit.Client.Models.ApiKeys;
using Emailit.Client.Models.Audiences;
using Emailit.Client.Models.Domains;
using Emailit.Client.Models.Emails;
using Emailit.Client.Models.Subscribers;
using Emailit.Client.Models.Suppressions;
using Emailit.Client.Models.Templates;
using Emailit.Client.Models.Verification;

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
        PropertyNameCaseInsensitive = true
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

    public async Task<bool> CancelEmailAsync(string emailId, CancellationToken ct = default)
    {
        try
        {
            var response = await ExecuteAsync<CancelEmailResponse>(
                () => _client.Request("/v2/emails", emailId, "cancel").PostAsync(cancellationToken: ct),
                ct);
            return response.Cancelled;
        }
        catch (EmailitException)
        {
            return false;
        }
    }

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
        }

        return ExecuteAsync<CursorPaginatedResponse<EmailResponse>>(
            () => req.GetAsync(cancellationToken: ct), ct);
    }

    public Task<EmailResponse> ResendEmailAsync(string emailId, CancellationToken ct = default) =>
        ExecuteAsync<EmailResponse>(
            () => _client.Request("/v2/emails", emailId, "resend").PostAsync(cancellationToken: ct),
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

    public Task<PaginatedResponse<SubscriberResponse>> ListSubscribersAsync(string audienceId, int page = 1, int limit = 100, CancellationToken ct = default) =>
        ExecuteAsync<PaginatedResponse<SubscriberResponse>>(
            () => _client.Request("/v2/audiences", audienceId, "subscribers")
                .SetQueryParams(new { page, limit })
                .GetAsync(cancellationToken: ct),
            ct);

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

    public Task<TemplateResponse> CreateTemplateAsync(CreateTemplateRequest request, CancellationToken ct = default) =>
        ExecuteAsync<TemplateResponse>(
            () => _client.Request("/v2/templates").PostJsonAsync(request, cancellationToken: ct),
            ct);

    public Task<TemplateResponse> GetTemplateAsync(string templateId, CancellationToken ct = default) =>
        ExecuteAsync<TemplateResponse>(
            () => _client.Request("/v2/templates", templateId).GetAsync(cancellationToken: ct),
            ct);

    public Task<PaginatedResponse<TemplateResponse>> ListTemplatesAsync(int page = 1, int limit = 100, CancellationToken ct = default) =>
        ExecuteAsync<PaginatedResponse<TemplateResponse>>(
            () => _client.Request("/v2/templates")
                .SetQueryParams(new { page, limit })
                .GetAsync(cancellationToken: ct),
            ct);

    public Task<TemplateResponse> UpdateTemplateAsync(string templateId, UpdateTemplateRequest request, CancellationToken ct = default) =>
        ExecuteAsync<TemplateResponse>(
            () => _client.Request("/v2/templates", templateId).PostJsonAsync(request, cancellationToken: ct),
            ct);

    public Task<TemplateResponse> PublishTemplateAsync(string templateId, CancellationToken ct = default) =>
        ExecuteAsync<TemplateResponse>(
            () => _client.Request("/v2/templates", templateId, "publish").PostAsync(cancellationToken: ct),
            ct);

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
                ?? throw new EmailitException("Failed to get export URL");
        }
        catch (FlurlHttpException ex)
        {
            await HandleErrorAsync(ex);
            throw; // unreachable — HandleErrorAsync always throws
        }
    }

    #endregion

    #region Connection Test

    public async Task<RateLimitInfo?> TestConnectionAsync(CancellationToken ct = default)
    {
        try
        {
            var response = await _client.Request("/v2/domains")
                .GetAsync(cancellationToken: ct);

            return ParseRateLimitInfo(response);
        }
        catch
        {
            return null;
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
            var rateLimitInfo = ParseRateLimitInfo(response);
            _lastRateLimitInfo = rateLimitInfo;

            using var stream = await response.GetStreamAsync();
            var result = await JsonSerializer.DeserializeAsync<T>(stream, JsonOptions, ct)
                ?? throw new EmailitException("Failed to deserialize response");

            if (result is EmailResponse emailResponse)
            {
                return (T)(object)(emailResponse with { RateLimitInfo = rateLimitInfo });
            }

            return result;
        }
        catch (FlurlHttpException ex)
        {
            await HandleErrorAsync(ex);
            throw;
        }
    }

    private static RateLimitInfo ParseRateLimitInfo(IFlurlResponse response) =>
        RateLimitInfo.FromHeaders(
            response.Headers.ToDictionary(
                h => h.Name,
                h => h.Value,
                StringComparer.OrdinalIgnoreCase));

    private async Task HandleErrorAsync(FlurlHttpException ex)
    {
        var statusCode = ex.StatusCode ?? 0;
        var body = await ex.GetResponseStringAsync() ?? "";

        ErrorResponse? errorResponse = null;
        try
        {
            errorResponse = JsonSerializer.Deserialize<ErrorResponse>(body, JsonOptions);
        }
        catch
        {
            // Ignore deserialization errors
        }

        var message = errorResponse?.GetErrorMessage() ?? ex.Message;

        RateLimitInfo? rateLimitInfo = null;
        if (ex.Call?.Response != null)
        {
            rateLimitInfo = ParseRateLimitInfo(ex.Call.Response);
            _lastRateLimitInfo = rateLimitInfo;
        }

        throw statusCode switch
        {
            400 => new EmailitValidationException(message, errorResponse?.Errors),
            401 => new EmailitAuthenticationException(message),
            403 => new EmailitException(message, 403),
            404 => new EmailitNotFoundException(message),
            413 => new EmailitMessageTooLargeException(errorResponse?.Details ?? message),
            429 when rateLimitInfo?.IsDailyLimitReached == true => new DailyLimitExceededException(rateLimitInfo),
            429 => new RateLimitExceededException(rateLimitInfo),
            >= 500 => new EmailitException($"Server error: {message}", statusCode),
            _ => new EmailitException(message, statusCode)
        };
    }

    #endregion
}

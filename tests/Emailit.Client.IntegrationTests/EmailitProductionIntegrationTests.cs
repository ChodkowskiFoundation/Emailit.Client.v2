using Emailit.Client.Exceptions;
using Emailit.Client.Models.ApiKeys;
using Emailit.Client.Models.Audiences;
using Emailit.Client.Models.Contacts;
using Emailit.Client.Models.Domains;
using Emailit.Client.Models.Emails;
using Emailit.Client.Models.Events;
using Emailit.Client.Models.Subscribers;
using Emailit.Client.Models.Suppressions;
using Emailit.Client.Models.Templates;
using Emailit.Client.Models.Verification;
using Emailit.Client.Models.Webhooks;

namespace Emailit.Client.IntegrationTests;

public sealed class EmailitProductionIntegrationTests
{
    [Fact]
    public async Task Exercises_All_Client_Endpoints_Against_Production()
    {
        var settings = IntegrationTestSettings.Load();
        using var client = new EmailitClient(new EmailitClientOptions
        {
            ApiKey = settings.ApiKey,
            BaseUrl = settings.BaseUrl,
            TimeoutSeconds = settings.TimeoutSeconds
        });

        var runId = $"{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid():N}"[..20];
        var managedDomainName = $"it-{runId}.{settings.SendingDomain}";
        var senderAddress = $"integration@{settings.SendingDomain}";
        var sender = $"Emailit Integration <{senderAddress}>";
        var contactEmail = $"contact-{runId}@{settings.SendingDomain}";
        var subscriberEmail = $"subscriber-{runId}@{settings.SendingDomain}";
        var suppressionEmail = $"suppression-{runId}@{settings.SendingDomain}";
        var retryProbeEmail = $"retry-{runId}@invalid.invalid";

        string? createdDomainId = null;
        string? createdApiKeyId = null;
        string? createdAudienceId = null;
        string? createdSubscriberId = null;
        string? createdTemplateId = null;
        string? createdSuppressionId = null;
        string? createdContactId = null;
        string? createdWebhookId = null;

        try
        {
            var rateLimitInfo = await client.TestConnectionAsync();
            rateLimitInfo.Should().NotBeNull();
            client.LastRateLimitInfo.Should().NotBeNull();

            var listedDomains = await client.ListDomainsAsync(limit: 100);
            listedDomains.Data.Should().NotBeNullOrEmpty();

            var baseDomain = listedDomains.Data.SingleOrDefault(x =>
                string.Equals(x.Name, settings.SendingDomain, StringComparison.OrdinalIgnoreCase));
            baseDomain.Should().NotBeNull($"configured sending domain {settings.SendingDomain} should exist");

            var fetchedBaseDomain = await client.GetDomainAsync(baseDomain!.Id);
            fetchedBaseDomain.Name.Should().Be(settings.SendingDomain);

            var createdDomain = await client.CreateDomainAsync(new CreateDomainRequest
            {
                Name = managedDomainName,
                TrackClicks = false,
                TrackLoads = false
            });
            createdDomainId = createdDomain.Id;
            createdDomain.Name.Should().Be(managedDomainName);

            var fetchedManagedDomain = await client.GetDomainAsync(createdDomainId);
            fetchedManagedDomain.Id.Should().Be(createdDomainId);

            var domainsAfterCreate = await client.ListDomainsAsync(limit: 100);
            domainsAfterCreate.Data.Should().Contain(x => x.Id == createdDomainId);

            var updatedDomain = await client.UpdateDomainAsync(createdDomainId, new UpdateDomainRequest
            {
                TrackClicks = true,
                TrackLoads = true,
                TrackingKey = $"go-{runId[..8]}",
                InboundKey = $"in-{runId[..8]}"
            });
            updatedDomain.Id.Should().Be(createdDomainId);

            await AssertVerifyDomainEndpointAsync(client, createdDomainId);

            var createdApiKey = await client.CreateApiKeyAsync(new CreateApiKeyRequest
            {
                Name = $"integration-{runId}",
                Scope = "full"
            });
            createdApiKeyId = createdApiKey.Id;
            createdApiKey.Key.Should().NotBeNullOrWhiteSpace();

            var fetchedApiKey = await client.GetApiKeyAsync(createdApiKeyId);
            fetchedApiKey.Id.Should().Be(createdApiKeyId);

            var apiKeys = await client.ListApiKeysAsync(limit: 100);
            apiKeys.Data.Should().Contain(x => x.Id == createdApiKeyId);

            var updatedApiKey = await client.UpdateApiKeyAsync(createdApiKeyId, new UpdateApiKeyRequest
            {
                Name = $"integration-updated-{runId}"
            });
            updatedApiKey.Name.Should().Contain("integration-updated");

            var createdAudience = await client.CreateAudienceAsync(new CreateAudienceRequest
            {
                Name = $"integration audience {runId}"
            });
            createdAudienceId = createdAudience.Id;

            var fetchedAudience = await client.GetAudienceAsync(createdAudienceId);
            fetchedAudience.Id.Should().Be(createdAudienceId);

            var listedAudiences = await client.ListAudiencesAsync(limit: 100);
            listedAudiences.Data.Should().Contain(x => x.Id == createdAudienceId);

            var updatedAudience = await client.UpdateAudienceAsync(createdAudienceId, new UpdateAudienceRequest
            {
                Name = $"integration audience updated {runId}"
            });
            updatedAudience.Name.Should().Contain("updated");

            var createdSubscriber = await client.AddSubscriberAsync(createdAudienceId, new AddSubscriberRequest
            {
                Email = subscriberEmail,
                FirstName = "Integration",
                LastName = "Subscriber",
                CustomFields = new Dictionary<string, object>
                {
                    ["run_id"] = runId
                }
            });
            createdSubscriberId = createdSubscriber.Id;

            var fetchedSubscriber = await client.GetSubscriberAsync(createdAudienceId, createdSubscriberId);
            fetchedSubscriber.Email.Should().Be(subscriberEmail);

            var listedSubscribers = await client.ListSubscribersAsync(createdAudienceId, limit: 100, subscribed: true);
            listedSubscribers.Data.Should().Contain(x => x.Id == createdSubscriberId);

            var updatedSubscriber = await client.UpdateSubscriberAsync(createdAudienceId, createdSubscriberId, new UpdateSubscriberRequest
            {
                FirstName = "Updated",
                Subscribed = true
            });
            updatedSubscriber.FirstName.Should().Be("Updated");

            var createdContact = await client.CreateContactAsync(new CreateContactRequest
            {
                Email = contactEmail,
                FirstName = "Integration",
                LastName = "Contact",
                CustomFields = new Dictionary<string, object>
                {
                    ["source"] = "integration"
                }
            });
            createdContactId = createdContact.Id;

            var fetchedContact = await client.GetContactAsync(createdContactId);
            fetchedContact.Email.Should().Be(contactEmail);

            var listedContacts = await client.ListContactsAsync(limit: 100);
            listedContacts.Data.Should().Contain(x => x.Id == createdContactId);

            var updatedContact = await client.UpdateContactAsync(createdContactId, new UpdateContactRequest
            {
                LastName = "Contact Updated",
                Unsubscribed = false
            });
            updatedContact.LastName.Should().Be("Contact Updated");

            var createdSuppression = await client.CreateSuppressionAsync(new CreateSuppressionRequest
            {
                Email = suppressionEmail,
                Type = "recipient",
                Reason = $"integration {runId}"
            });
            createdSuppressionId = createdSuppression.Id;

            var fetchedSuppression = await client.GetSuppressionAsync(createdSuppressionId);
            fetchedSuppression.Email.Should().Be(suppressionEmail);

            var listedSuppressions = await client.ListSuppressionsAsync(limit: 100);
            listedSuppressions.Data.Should().Contain(x => x.Id == createdSuppressionId);

            var updatedSuppression = await client.UpdateSuppressionAsync(createdSuppressionId, new UpdateSuppressionRequest
            {
                Reason = $"integration updated {runId}",
                Type = "recipient"
            });
            updatedSuppression.Reason.Should().Contain("updated");

            var createdTemplate = await client.CreateTemplateAsync(new CreateTemplateRequest
            {
                Name = $"integration-template-{runId}",
                Alias = $"integration-template-{runId}",
                From = senderAddress,
                Subject = $"Integration template {runId}",
                Html = $"<p>Template integration test {runId}</p>",
                Text = $"Template integration test {runId}",
                Editor = "html"
            });
            createdTemplateId = createdTemplate.Id;

            var fetchedTemplate = await client.GetTemplateAsync(createdTemplateId);
            fetchedTemplate.Id.Should().Be(createdTemplateId);

            var listedTemplates = await client.ListTemplatesAsync(new ListTemplatesRequest
            {
                FilterAlias = $"integration-template-{runId}",
                Page = 1,
                PerPage = 25
            });
            listedTemplates.Data.Should().Contain(x => x.Id == createdTemplateId);

            var updatedTemplate = await client.UpdateTemplateAsync(createdTemplateId, new UpdateTemplateRequest
            {
                Subject = $"Integration template updated {runId}",
                Html = $"<p>Updated template integration test {runId}</p>"
            });
            updatedTemplate.Subject.Should().Contain("updated");

            var publishedTemplate = await client.PublishTemplateAsync(createdTemplateId);
            publishedTemplate.Id.Should().Be(createdTemplateId);

            var createdWebhook = await client.CreateWebhookAsync(new CreateWebhookRequest
            {
                Name = $"integration-webhook-{runId}",
                Url = $"https://{settings.SendingDomain}/emailit-integration/{runId}",
                Enabled = true,
                AllEvents = true
            });
            createdWebhookId = createdWebhook.Id;

            var listedWebhooks = await PollAsync(
                async () => await client.ListWebhooksAsync(limit: 100),
                webhooks => webhooks.Data.Any(x => x.Name == createdWebhook.Name),
                TimeSpan.FromMinutes(1),
                TimeSpan.FromSeconds(3));
            var listedWebhook = listedWebhooks.Data.Single(x => x.Name == createdWebhook.Name);
            createdWebhookId = listedWebhook.Id;
            listedWebhook.Url.Should().Be($"https://{settings.SendingDomain}/emailit-integration/{runId}");

            var fetchedWebhook = await client.GetWebhookAsync(createdWebhookId);
            fetchedWebhook.Id.Should().Be(createdWebhookId);

            var updatedWebhook = await client.UpdateWebhookAsync(createdWebhookId, new UpdateWebhookRequest
            {
                Name = $"integration-webhook-updated-{runId}",
                Enabled = false
            });
            updatedWebhook.Name.Should().Contain("updated");

            var verifiedEmail = await client.VerifyEmailAsync(new VerifyEmailRequest
            {
                Email = settings.RecipientEmail,
                Mode = "default"
            });
            verifiedEmail.Email.Should().Be(settings.RecipientEmail);

            var verificationList = await client.CreateVerificationListAsync(new CreateVerificationListRequest
            {
                Name = $"integration-verification-{runId}",
                Emails = [settings.RecipientEmail, suppressionEmail]
            });

            var fetchedVerificationList = await client.GetVerificationListAsync(verificationList.Id);
            fetchedVerificationList.Id.Should().Be(verificationList.Id);

            var listedVerificationLists = await client.ListVerificationListsAsync(limit: 100);
            listedVerificationLists.Data.Should().Contain(x => x.Id == verificationList.Id);

            var verificationResults = await PollAsync(
                async () => await client.GetVerificationResultsAsync(verificationList.Id, limit: 100),
                results => results.Data.Count > 0,
                TimeSpan.FromMinutes(2),
                TimeSpan.FromSeconds(5));
            verificationResults.ListId.Should().Be(verificationList.Id);

            var exportUrl = await client.ExportVerificationResultsAsync(verificationList.Id);
            exportUrl.Should().NotBeNullOrWhiteSpace();

            var liveEmail = await client.SendEmailAsync(new SendEmailRequest
            {
                From = sender,
                To = [settings.RecipientEmail],
                Subject = $"Integration live email {runId}",
                Html = $"<p>Testing send/get/list/meta/body/raw/attachments for run {runId}.</p>",
                Text = $"Testing send/get/list/meta/body/raw/attachments for run {runId}.",
                Meta = new Dictionary<string, string>
                {
                    ["run_id"] = runId,
                    ["test"] = "live-email"
                },
                Headers = new Dictionary<string, string>
                {
                    ["X-Emailit-Integration"] = runId
                },
                Attachments =
                [
                    new EmailAttachment
                    {
                        Filename = $"integration-{runId}.txt",
                        Content = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"attachment for {runId}")),
                        ContentType = "text/plain"
                    }
                ]
            }, idempotencyKey: $"integration-live-{runId}");
            liveEmail.Id.Should().NotBeNullOrWhiteSpace();

            var fetchedLiveEmail = await PollAsync(
                async () => await client.GetEmailAsync(liveEmail.Id),
                email => !string.IsNullOrWhiteSpace(email.Id),
                TimeSpan.FromMinutes(1),
                TimeSpan.FromSeconds(3));
            fetchedLiveEmail.Subject.Should().Be($"Integration live email {runId}");

            var listedEmails = await client.ListEmailsAsync(new ListEmailsRequest
            {
                Subject = $"Integration live email {runId}",
                Limit = 20
            });
            listedEmails.Data.Should().Contain(x => x.Id == liveEmail.Id);

            var emailMeta = await PollAsync(
                async () => await client.GetEmailMetaAsync(liveEmail.Id),
                meta => meta.Attachments is { Count: > 0 },
                TimeSpan.FromMinutes(1),
                TimeSpan.FromSeconds(3));
            emailMeta.Id.Should().Be(liveEmail.Id);

            var emailBody = await client.GetEmailBodyAsync(liveEmail.Id);
            (emailBody.Html ?? emailBody.Text).Should().Contain(runId);

            var emailRaw = await client.GetEmailRawAsync(liveEmail.Id);
            emailRaw.Raw.Should().Contain($"Integration live email {runId}");

            var emailAttachments = await client.GetEmailAttachmentsAsync(liveEmail.Id);
            emailAttachments.Attachments.Should().ContainSingle();

            var scheduledAt = DateTimeOffset.UtcNow.AddMinutes(20);
            var updatedScheduledAt = scheduledAt.AddMinutes(10);
            var scheduledEmail = await client.SendEmailAsync(new SendEmailRequest
            {
                From = sender,
                To = [settings.RecipientEmail],
                Subject = $"Integration scheduled email {runId}",
                Html = $"<p>Testing update/cancel of scheduled email for run {runId}.</p>",
                Text = $"Testing update/cancel of scheduled email for run {runId}.",
                ScheduledAt = scheduledAt.ToString("O")
            }, idempotencyKey: $"integration-scheduled-{runId}");
            scheduledEmail.Status.Should().NotBeNullOrWhiteSpace();

            var updatedScheduledEmail = await client.UpdateScheduledEmailAsync(scheduledEmail.Id, new UpdateScheduledEmailRequest
            {
                ScheduledAt = updatedScheduledAt.ToString("O")
            });
            updatedScheduledEmail.Id.Should().Be(scheduledEmail.Id);

            var canceledEmail = await client.CancelEmailAsync(scheduledEmail.Id);
            canceledEmail.Id.Should().Be(scheduledEmail.Id);

            var retryCandidateId = await GetRetryCandidateAsync(client, sender, retryProbeEmail, runId);

#pragma warning disable CS0618
            var resentEmail = await client.ResendEmailAsync(retryCandidateId);
#pragma warning restore CS0618
            resentEmail.Id.Should().NotBeNullOrWhiteSpace();

            var retriedEmail = await client.RetryEmailAsync(retryCandidateId);
            retriedEmail.Id.Should().NotBeNullOrWhiteSpace();

            var listedEvents = await PollAsync(
                async () => await client.ListEventsAsync(new ListEventsRequest
                {
                    Page = 1,
                    Limit = 50,
                    IncludeData = true
                }),
                eventsPage => eventsPage.Data.Count > 0,
                TimeSpan.FromMinutes(1),
                TimeSpan.FromSeconds(3));
            listedEvents.Data.Should().NotBeEmpty();

            var fetchedEvent = await client.GetEventAsync(listedEvents.Data[0].Id);
            fetchedEvent.Id.Should().Be(listedEvents.Data[0].Id);

            var deletedWebhook = await client.DeleteWebhookAsync(createdWebhookId);
            deletedWebhook.Deleted.Should().BeTrue();
            deletedWebhook.Id.Should().Be(createdWebhookId);
            createdWebhookId = null;

            var deletedSuppression = await client.DeleteSuppressionAsync(createdSuppressionId);
            deletedSuppression.Deleted.Should().BeTrue();
            deletedSuppression.Id.Should().Be(createdSuppressionId);
            createdSuppressionId = null;

            var deletedContact = await client.DeleteContactAsync(createdContactId);
            deletedContact.Deleted.Should().BeTrue();
            deletedContact.Id.Should().Be(createdContactId);
            createdContactId = null;

            var deletedSubscriber = await client.DeleteSubscriberAsync(createdAudienceId, createdSubscriberId);
            deletedSubscriber.Deleted.Should().BeTrue();
            deletedSubscriber.Id.Should().Be(createdSubscriberId);
            createdSubscriberId = null;

            var deletedAudience = await client.DeleteAudienceAsync(createdAudienceId);
            deletedAudience.Deleted.Should().BeTrue();
            deletedAudience.Id.Should().Be(createdAudienceId);
            createdAudienceId = null;

            var deletedTemplate = await client.DeleteTemplateAsync(createdTemplateId);
            deletedTemplate.Message.Should().NotBeNullOrWhiteSpace();
            createdTemplateId = null;

            var deletedApiKey = await client.DeleteApiKeyAsync(createdApiKeyId);
            deletedApiKey.Deleted.Should().BeTrue();
            deletedApiKey.Id.Should().Be(createdApiKeyId);
            createdApiKeyId = null;

            var deletedDomain = await client.DeleteDomainAsync(createdDomainId);
            deletedDomain.Deleted.Should().BeTrue();
            deletedDomain.Id.Should().Be(createdDomainId);
            createdDomainId = null;
        }
        finally
        {
            await TryDeleteAsync(() => createdWebhookId is null
                ? Task.FromResult(false)
                : DeleteAndConfirmAsync(() => client.DeleteWebhookAsync(createdWebhookId), () => createdWebhookId = null));

            await TryDeleteAsync(() => createdSuppressionId is null
                ? Task.FromResult(false)
                : DeleteAndConfirmAsync(() => client.DeleteSuppressionAsync(createdSuppressionId), () => createdSuppressionId = null));

            await TryDeleteAsync(() => createdContactId is null
                ? Task.FromResult(false)
                : DeleteAndConfirmAsync(() => client.DeleteContactAsync(createdContactId), () => createdContactId = null));

            await TryDeleteAsync(() => createdSubscriberId is null || createdAudienceId is null
                ? Task.FromResult(false)
                : DeleteAndConfirmAsync(() => client.DeleteSubscriberAsync(createdAudienceId, createdSubscriberId), () => createdSubscriberId = null));

            await TryDeleteAsync(() => createdAudienceId is null
                ? Task.FromResult(false)
                : DeleteAndConfirmAsync(() => client.DeleteAudienceAsync(createdAudienceId), () => createdAudienceId = null));

            await TryDeleteAsync(() => createdTemplateId is null
                ? Task.FromResult(false)
                : DeleteAndConfirmAsync(() => client.DeleteTemplateAsync(createdTemplateId), () => createdTemplateId = null));

            await TryDeleteAsync(() => createdApiKeyId is null
                ? Task.FromResult(false)
                : DeleteAndConfirmAsync(() => client.DeleteApiKeyAsync(createdApiKeyId), () => createdApiKeyId = null));

            await TryDeleteAsync(() => createdDomainId is null
                ? Task.FromResult(false)
                : DeleteAndConfirmAsync(() => client.DeleteDomainAsync(createdDomainId), () => createdDomainId = null));
        }
    }

    private static async Task AssertVerifyDomainEndpointAsync(EmailitClient client, string domainId)
    {
        try
        {
            var verifyResult = await client.VerifyDomainAsync(domainId);
            verifyResult.Id.Should().Be(domainId);
        }
        catch (EmailitValidationException ex)
        {
            ex.Message.Should().NotBeNullOrWhiteSpace();
        }
    }

    private static async Task<string> GetRetryCandidateAsync(
        EmailitClient client,
        string sender,
        string retryProbeEmail,
        string runId)
    {
        var existing = await FindRetryCandidateFromHistoryAsync(client);
        if (existing is not null)
        {
            return existing;
        }

        var probeSubject = $"Integration retry probe {runId}";

        try
        {
            var probeEmail = await client.SendEmailAsync(new SendEmailRequest
            {
                From = sender,
                To = [retryProbeEmail],
                Subject = probeSubject,
                Html = $"<p>Testing retry/resend endpoints for run {runId}.</p>",
                Text = $"Testing retry/resend endpoints for run {runId}."
            }, idempotencyKey: $"integration-retry-probe-{runId}");

            var failedProbe = await PollAsync(
                async () => await client.GetEmailAsync(probeEmail.Id),
                email => IsRetryableStatus(email.Status),
                TimeSpan.FromMinutes(2),
                TimeSpan.FromSeconds(5));

            return failedProbe.Id;
        }
        catch (EmailitException)
        {
            existing = await FindRetryCandidateFromHistoryAsync(client);
            if (existing is not null)
            {
                return existing;
            }

            throw new InvalidOperationException(
                "Could not obtain a retryable email for RetryEmailAsync/ResendEmailAsync. " +
                "The account needs at least one failed, bounced, or rejected email.");
        }
    }

    private static async Task<string?> FindRetryCandidateFromHistoryAsync(EmailitClient client)
    {
        foreach (var status in new[] { "failed", "bounced", "rejected" })
        {
            var page = await client.ListEmailsAsync(new ListEmailsRequest
            {
                Status = status,
                Limit = 20
            });

            var match = page.Data.FirstOrDefault();
            if (match is not null)
            {
                return match.Id;
            }
        }

        return null;
    }

    private static bool IsRetryableStatus(string? status) =>
        string.Equals(status, "failed", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(status, "bounced", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(status, "rejected", StringComparison.OrdinalIgnoreCase);

    private static async Task<T> PollAsync<T>(
        Func<Task<T>> action,
        Func<T, bool> predicate,
        TimeSpan timeout,
        TimeSpan interval)
    {
        var started = DateTimeOffset.UtcNow;
        Exception? lastError = null;

        while (DateTimeOffset.UtcNow - started < timeout)
        {
            try
            {
                var result = await action();
                if (predicate(result))
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                lastError = ex;
            }

            await Task.Delay(interval);
        }

        if (lastError is not null)
        {
            throw new TimeoutException("Polling timed out.", lastError);
        }

        throw new TimeoutException("Polling timed out before the expected condition was met.");
    }

    private static async Task TryDeleteAsync(Func<Task<bool>> deleteAction)
    {
        try
        {
            await deleteAction();
        }
        catch
        {
            // Best-effort cleanup after production integration tests.
        }
    }

    private static async Task<bool> DeleteAndConfirmAsync<T>(
        Func<Task<T>> deleteAction,
        Action onSuccess)
    {
        await deleteAction();
        onSuccess();
        return true;
    }
}

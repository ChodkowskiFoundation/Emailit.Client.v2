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
    public async Task Connection_Domains_And_ApiKeys_Are_Production_Compatible()
    {
        var settings = IntegrationTestSettings.Load();
        using var client = CreateClient(settings);
        await using var cleanup = new CleanupScope();

        var runId = NewRunId();
        var managedDomainName = $"it-{runId}.{settings.SendingDomain}";

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
        cleanup.Add(async () => await client.DeleteDomainAsync(createdDomain.Id));
        createdDomain.Name.Should().Be(managedDomainName);

        var fetchedManagedDomain = await client.GetDomainAsync(createdDomain.Id);
        fetchedManagedDomain.Id.Should().Be(createdDomain.Id);

        var domainsAfterCreate = await client.ListDomainsAsync(limit: 100);
        domainsAfterCreate.Data.Should().Contain(x => x.Id == createdDomain.Id);

        var updatedDomain = await client.UpdateDomainAsync(createdDomain.Id, new UpdateDomainRequest
        {
            TrackClicks = true,
            TrackLoads = true,
            TrackingKey = $"go-{runId[..8]}",
            InboundKey = $"in-{runId[..8]}"
        });
        updatedDomain.Id.Should().Be(createdDomain.Id);

        await AssertVerifyDomainEndpointAsync(client, createdDomain.Id);

        var createdApiKey = await client.CreateApiKeyAsync(new CreateApiKeyRequest
        {
            Name = $"integration-{runId}",
            Scope = "full"
        });
        cleanup.Add(async () => await client.DeleteApiKeyAsync(createdApiKey.Id));
        createdApiKey.Key.Should().NotBeNullOrWhiteSpace();

        var fetchedApiKey = await client.GetApiKeyAsync(createdApiKey.Id);
        fetchedApiKey.Id.Should().Be(createdApiKey.Id);

        var apiKeys = await client.ListApiKeysAsync(limit: 100);
        apiKeys.Data.Should().Contain(x => x.Id == createdApiKey.Id);

        var updatedApiKey = await client.UpdateApiKeyAsync(createdApiKey.Id, new UpdateApiKeyRequest
        {
            Name = $"integration-updated-{runId}"
        });
        updatedApiKey.Name.Should().Contain("integration-updated");
    }

    [Fact]
    public async Task Audiences_Subscribers_Contacts_And_Suppressions_Are_Production_Compatible()
    {
        var settings = IntegrationTestSettings.Load();
        using var client = CreateClient(settings);
        await using var cleanup = new CleanupScope();

        var runId = NewRunId();
        var subscriberEmail = $"subscriber-{runId}@{settings.SendingDomain}";
        var contactEmail = $"contact-{runId}@{settings.SendingDomain}";
        var suppressionEmail = $"suppression-{runId}@{settings.SendingDomain}";

        var createdAudience = await client.CreateAudienceAsync(new CreateAudienceRequest
        {
            Name = $"integration audience {runId}"
        });
        cleanup.Add(async () => await client.DeleteAudienceAsync(createdAudience.Id));

        var fetchedAudience = await client.GetAudienceAsync(createdAudience.Id);
        fetchedAudience.Id.Should().Be(createdAudience.Id);

        var listedAudiences = await client.ListAudiencesAsync(limit: 100);
        listedAudiences.Data.Should().Contain(x => x.Id == createdAudience.Id);

        var updatedAudience = await client.UpdateAudienceAsync(createdAudience.Id, new UpdateAudienceRequest
        {
            Name = $"integration audience updated {runId}"
        });
        updatedAudience.Name.Should().Contain("updated");

        var createdSubscriber = await client.AddSubscriberAsync(createdAudience.Id, new AddSubscriberRequest
        {
            Email = subscriberEmail,
            FirstName = "Integration",
            LastName = "Subscriber",
            CustomFields = new Dictionary<string, object>
            {
                ["run_id"] = runId
            }
        });
        cleanup.Add(async () => await client.DeleteSubscriberAsync(createdAudience.Id, createdSubscriber.Id));

        var fetchedSubscriber = await client.GetSubscriberAsync(createdAudience.Id, createdSubscriber.Id);
        fetchedSubscriber.Email.Should().Be(subscriberEmail);

        var listedSubscribers = await client.ListSubscribersAsync(createdAudience.Id, limit: 100, subscribed: true);
        listedSubscribers.Data.Should().Contain(x => x.Id == createdSubscriber.Id);

        var updatedSubscriber = await client.UpdateSubscriberAsync(createdAudience.Id, createdSubscriber.Id, new UpdateSubscriberRequest
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
        cleanup.Add(async () => await client.DeleteContactAsync(createdContact.Id));

        var fetchedContact = await client.GetContactAsync(createdContact.Id);
        fetchedContact.Email.Should().Be(contactEmail);

        var listedContacts = await client.ListContactsAsync(limit: 100);
        listedContacts.Data.Should().Contain(x => x.Id == createdContact.Id);

        var updatedContact = await client.UpdateContactAsync(createdContact.Id, new UpdateContactRequest
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
        cleanup.Add(async () => await client.DeleteSuppressionAsync(createdSuppression.Id));

        var fetchedSuppression = await client.GetSuppressionAsync(createdSuppression.Id);
        fetchedSuppression.Email.Should().Be(suppressionEmail);

        var listedSuppressions = await client.ListSuppressionsAsync(limit: 100);
        listedSuppressions.Data.Should().Contain(x => x.Id == createdSuppression.Id);

        var updatedSuppression = await client.UpdateSuppressionAsync(createdSuppression.Id, new UpdateSuppressionRequest
        {
            Reason = $"integration updated {runId}",
            Type = "recipient"
        });
        updatedSuppression.Reason.Should().Contain("updated");
    }

    [Fact]
    public async Task Templates_Are_Production_Compatible()
    {
        var settings = IntegrationTestSettings.Load();
        using var client = CreateClient(settings);
        await using var cleanup = new CleanupScope();

        var runId = NewRunId();
        var senderAddress = $"integration@{settings.SendingDomain}";

        var createdTemplate = await client.CreateTemplateAsync(new CreateTemplateRequest
        {
            Name = $"integration-template-{runId}",
            Alias = $"integration-template-{runId}",
            From = senderAddress,
            Subject = $"Integration template {runId}",
            Html = $"<p>Testing templates for run {runId}.</p>",
            Text = $"Testing templates for run {runId}.",
            Editor = "html"
        });
        cleanup.Add(async () => await client.DeleteTemplateAsync(createdTemplate.Id));

        var fetchedTemplate = await client.GetTemplateAsync(createdTemplate.Id);
        fetchedTemplate.Id.Should().Be(createdTemplate.Id);

        var listedTemplates = await client.ListTemplatesAsync(new ListTemplatesRequest
        {
            FilterAlias = $"integration-template-{runId}",
            Page = 1,
            PerPage = 25
        });
        listedTemplates.Data.Should().Contain(x => x.Id == createdTemplate.Id);

        var updatedTemplate = await client.UpdateTemplateAsync(createdTemplate.Id, new UpdateTemplateRequest
        {
            Subject = $"Integration template updated {runId}",
            Html = $"<p>Updated template integration test {runId}.</p>"
        });
        updatedTemplate.Subject.Should().Contain("updated");

        var publishedTemplate = await client.PublishTemplateAsync(createdTemplate.Id);
        publishedTemplate.Id.Should().Be(createdTemplate.Id);
    }

    [Fact]
    public async Task Emails_And_Subresources_Are_Production_Compatible()
    {
        var settings = IntegrationTestSettings.Load();
        using var client = CreateClient(settings);

        var runId = NewRunId();
        var senderAddress = $"integration@{settings.SendingDomain}";
        var sender = $"Emailit Integration <{senderAddress}>";

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
            meta => meta.Id == liveEmail.Id,
            TimeSpan.FromMinutes(1),
            TimeSpan.FromSeconds(3));
        emailMeta.Id.Should().Be(liveEmail.Id);

        var emailBody = await client.GetEmailBodyAsync(liveEmail.Id);
        (emailBody.Html ?? emailBody.Text).Should().Contain(runId);

        var emailRaw = await client.GetEmailRawAsync(liveEmail.Id);
        emailRaw.Raw.Should().Contain($"Integration live email {runId}");

        var emailAttachments = await client.GetEmailAttachmentsAsync(liveEmail.Id);
        emailAttachments.Should().NotBeNull();
        emailAttachments.Attachments.Should().NotBeNull();

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
        canceledEmail.Status.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    [Trait("Stability", "Unstable")]
    public async Task Retry_And_Resend_Are_Production_Compatible_When_A_Retryable_Email_Exists()
    {
        var settings = IntegrationTestSettings.Load();
        if (!RequireUnstableEndpointsEnabled(settings, "retry/resend routes are inconsistent in production"))
        {
            return;
        }

        using var client = CreateClient(settings);

        var runId = NewRunId();
        var sender = $"Emailit Integration <integration@{settings.SendingDomain}>";
        var retryProbeEmail = $"retry-{runId}@invalid.invalid";

        var retryCandidateId = await GetRetryCandidateAsync(client, sender, retryProbeEmail, runId);
        if (string.IsNullOrWhiteSpace(retryCandidateId))
        {
            return;
        }

#pragma warning disable CS0618
        var resentEmail = await client.ResendEmailAsync(retryCandidateId);
#pragma warning restore CS0618
        resentEmail.Id.Should().NotBeNullOrWhiteSpace();

        var retriedEmail = await client.RetryEmailAsync(retryCandidateId);
        retriedEmail.Id.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Events_Are_Production_Compatible()
    {
        var settings = IntegrationTestSettings.Load();
        using var client = CreateClient(settings);

        var listedEvents = await PollAsync(
            async () => await client.ListEventsAsync(new ListEventsRequest
            {
                Page = 1,
                Limit = 50,
                IncludeData = true
            }),
            page => page.Data.Count > 0,
            TimeSpan.FromMinutes(1),
            TimeSpan.FromSeconds(3));
        listedEvents.Data.Should().NotBeEmpty();

        var fetchedEvent = await client.GetEventAsync(listedEvents.Data[0].Id);
        fetchedEvent.Id.Should().Be(listedEvents.Data[0].Id);
    }

    [Fact]
    public async Task Verify_Email_Is_Production_Compatible()
    {
        var settings = IntegrationTestSettings.Load();
        using var client = CreateClient(settings);

        var verifiedEmail = await client.VerifyEmailAsync(new VerifyEmailRequest
        {
            Email = settings.RecipientEmail
        });
        verifiedEmail.Email.Should().Be(settings.RecipientEmail);
    }

    [Fact]
    [Trait("Stability", "Unstable")]
    public async Task Verification_List_Endpoints_Are_Production_Compatible_When_Unstable_Tests_Are_Enabled()
    {
        var settings = IntegrationTestSettings.Load();
        if (!RequireUnstableEndpointsEnabled(settings, "verification list endpoints may return 500 in production"))
        {
            return;
        }

        using var client = CreateClient(settings);

        var runId = NewRunId();
        var suppressionEmail = $"suppression-{runId}@{settings.SendingDomain}";

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
    }

    [Fact]
    [Trait("Stability", "Unstable")]
    public async Task Webhook_Endpoints_Are_Production_Compatible_When_Unstable_Tests_Are_Enabled()
    {
        var settings = IntegrationTestSettings.Load();
        if (!RequireUnstableEndpointsEnabled(settings, "webhook endpoints are inconsistent in production"))
        {
            return;
        }

        using var client = CreateClient(settings);
        await using var cleanup = new CleanupScope();

        var runId = NewRunId();
        var createdWebhook = await client.CreateWebhookAsync(new CreateWebhookRequest
        {
            Name = $"integration-webhook-{runId}",
            Url = $"https://{settings.SendingDomain}/emailit-integration/{runId}",
            Enabled = true,
            AllEvents = true
        });

        var listedWebhooks = await PollAsync(
            async () => await client.ListWebhooksAsync(limit: 100),
            webhooks => webhooks.Data.Any(x => x.Name == createdWebhook.Name),
            TimeSpan.FromMinutes(1),
            TimeSpan.FromSeconds(3));
        var listedWebhook = listedWebhooks.Data.Single(x => x.Name == createdWebhook.Name);
        cleanup.Add(async () => await client.DeleteWebhookAsync(listedWebhook.Id));
        listedWebhook.Url.Should().Be($"https://{settings.SendingDomain}/emailit-integration/{runId}");

        var fetchedWebhook = await client.GetWebhookAsync(listedWebhook.Id);
        fetchedWebhook.Id.Should().Be(listedWebhook.Id);

        var updatedWebhook = await client.UpdateWebhookAsync(listedWebhook.Id, new UpdateWebhookRequest
        {
            Name = $"integration-webhook-updated-{runId}",
            Enabled = false
        });
        updatedWebhook.Name.Should().Contain("updated");
    }

    private static EmailitClient CreateClient(IntegrationTestSettings settings) => new(new EmailitClientOptions
    {
        ApiKey = settings.ApiKey,
        BaseUrl = settings.BaseUrl,
        TimeoutSeconds = settings.TimeoutSeconds
    });

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

        try
        {
            var probeEmail = await client.SendEmailAsync(new SendEmailRequest
            {
                From = sender,
                To = [retryProbeEmail],
                Subject = $"Integration retry probe {runId}",
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

            return string.Empty;
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

    private static bool RequireUnstableEndpointsEnabled(IntegrationTestSettings settings, string reason)
    {
        if (settings.EnableUnstableEndpoints)
        {
            return true;
        }

        Console.WriteLine(
            $"Skipping unstable integration test. Set EMAILIT_INTEGRATION_ENABLE_UNSTABLE=true to run it. Reason: {reason}.");
        return false;
    }

    private static string NewRunId() => $"{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid():N}"[..20];

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

    private sealed class CleanupScope : IAsyncDisposable
    {
        private readonly Stack<Func<Task>> _cleanupActions = new();

        public void Add(Func<Task> cleanup) => _cleanupActions.Push(cleanup);

        public async ValueTask DisposeAsync()
        {
            while (_cleanupActions.Count > 0)
            {
                var cleanup = _cleanupActions.Pop();

                try
                {
                    await cleanup();
                }
                catch
                {
                    // Best-effort cleanup after production integration tests.
                }
            }
        }
    }
}

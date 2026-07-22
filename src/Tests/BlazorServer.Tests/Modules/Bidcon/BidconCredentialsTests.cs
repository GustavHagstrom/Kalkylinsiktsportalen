using BlazorServer.Modules.Bidcon.Features.GetBidconCredentials;
using BlazorServer.Modules.Bidcon.Features.RemoveBidconCredentials;
using BlazorServer.Modules.Bidcon.Features.UpsertBidconCredentials;
using BlazorServer.Modules.Bidcon.CredentialAdministration;
using BlazorServer.Modules.Bidcon.AccessTokens;
using BlazorServer.Modules.Bidcon.Infrastructure.Persistence;
using BlazorServer.Kernel.Validation;
using BlazorServer.Modules.Bidcon.Public.Contracts;
using BlazorServer.Identity;

namespace BlazorServer.Tests.Modules.Bidcon;

[TestFixture]
public class BidconCredentialsTests
{
    [Test]
    public async Task GetBidconCredentials_ReturnsNotConfigured_WhenCredentialsAreMissing()
    {
        var organizationId = new OrganizationId("1c928774-3e22-4a94-9146-8784fb54d67a");
        var handler = new GetBidconCredentialsQueryHandler(new RecordingBidconApiCredentialsStore());

        var result = await handler.HandleAsync(
            new GetBidconCredentialsQuery(organizationId),
            CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<BidconCredentialsNotConfigured>());
            var notConfigured = (BidconCredentialsNotConfigured)result;
            Assert.That(notConfigured.OrganizationId, Is.EqualTo(organizationId));
            Assert.That(notConfigured.IsConfigured, Is.False);
            Assert.That(notConfigured.ApiKey, Is.Empty);
            Assert.That(notConfigured.ApiSecret, Is.Empty);
        });
    }

    [Test]
    public async Task GetBidconCredentials_ReturnsMaskedConfiguredCredentials_WhenCredentialsExist()
    {
        var organizationId = new OrganizationId("1c928774-3e22-4a94-9146-8784fb54d67a");
        var store = new RecordingBidconApiCredentialsStore();
        await store.UpsertAsync(new BidconApiCredentials
        {
            OrganizationId = organizationId,
            ApiKey = "key-1",
            ApiSecret = "secret-1"
        });

        var handler = new GetBidconCredentialsQueryHandler(store);

        var result = await handler.HandleAsync(
            new GetBidconCredentialsQuery(organizationId),
            CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<BidconCredentialsFound>());
            var found = (BidconCredentialsFound)result;
            Assert.That(found.OrganizationId, Is.EqualTo(organizationId));
            Assert.That(found.IsConfigured, Is.True);
            Assert.That(found.ApiKey, Is.EqualTo("****ey-1"));
            Assert.That(found.ApiSecret, Is.EqualTo("****et-1"));
        });
    }

    [Test]
    public async Task GetBidconCredentials_ReturnsValidationError_WhenOrganizationIsMissing()
    {
        var store = new RecordingBidconApiCredentialsStore();
        var handler = new GetBidconCredentialsQueryHandler(store);

        var result = await handler.HandleAsync(
            new GetBidconCredentialsQuery(new OrganizationId(string.Empty)),
            CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<InvalidGetBidconCredentialsRequest>());
            var invalidRequest = (InvalidGetBidconCredentialsRequest)result;
            Assert.That(invalidRequest.ValidationErrors, Has.One.Matches<ValidationError>(error =>
                error.Code == "bidcon.credentials.organization.required" &&
                error.Field == nameof(GetBidconCredentialsQuery.OrganizationId)));
            Assert.That(store.WasRead, Is.False);
        });
    }

    [Test]
    public async Task UpsertBidconCredentials_SavesCredentials_WhenCommandIsValid()
    {
        var organizationId = new OrganizationId("1c928774-3e22-4a94-9146-8784fb54d67a");
        var actorId = new UserId("admin-1");
        var store = new RecordingBidconApiCredentialsStore();
        var verifier = new RecordingBidconCredentialsVerifier(true);
        var tokenCache = new RecordingBidconAccessTokenCache();
        var auditLog = new RecordingBidconCredentialsAuditLog();
        var handler = new UpsertBidconCredentialsCommandHandler(store, verifier, tokenCache, auditLog);

        var result = await handler.HandleAsync(
            new UpsertBidconCredentialsCommand(organizationId, actorId, "key-1", "secret-1"),
            CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<BidconCredentialsSaved>());
            var saved = (BidconCredentialsSaved)result;
            Assert.That(saved.OrganizationId, Is.EqualTo(organizationId));
            Assert.That(saved.WasSaved, Is.True);
            Assert.That(store.SavedCredentials, Is.Not.Null);
            Assert.That(store.SavedCredentials!.OrganizationId, Is.EqualTo(organizationId));
            Assert.That(store.SavedCredentials.ApiKey, Is.EqualTo("key-1"));
            Assert.That(store.SavedCredentials.ApiSecret, Is.EqualTo("secret-1"));
            Assert.That(verifier.VerifiedCredentials, Is.Not.Null);
            Assert.That(verifier.VerifiedCredentials!.ApiKey, Is.EqualTo("key-1"));
            Assert.That(verifier.VerifiedCredentials.ApiSecret, Is.EqualTo("secret-1"));
            Assert.That(tokenCache.InvalidatedOrganizationId, Is.EqualTo(organizationId));
        });
    }

    [Test]
    public async Task UpsertBidconCredentials_KeepsExistingCredentials_WhenVerificationFails()
    {
        var organizationId = new OrganizationId("1c928774-3e22-4a94-9146-8784fb54d67a");
        var actorId = new UserId("admin-1");
        var store = new RecordingBidconApiCredentialsStore();
        await store.UpsertAsync(new BidconApiCredentials
        {
            OrganizationId = organizationId,
            ApiKey = "old-key",
            ApiSecret = "old-secret"
        });

        var tokenCache = new RecordingBidconAccessTokenCache();
        var handler = new UpsertBidconCredentialsCommandHandler(
            store,
            new RecordingBidconCredentialsVerifier(false),
            tokenCache,
            new RecordingBidconCredentialsAuditLog());

        var result = await handler.HandleAsync(
            new UpsertBidconCredentialsCommand(organizationId, actorId, "new-key", "new-secret"),
            CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<BidconCredentialsVerificationFailed>());
            var verificationFailed = (BidconCredentialsVerificationFailed)result;
            Assert.That(verificationFailed.OrganizationId, Is.EqualTo(organizationId));
            Assert.That(verificationFailed.Message, Is.EqualTo("Bidcon credentials could not be verified."));
            Assert.That(store.SavedCredentials!.ApiKey, Is.EqualTo("old-key"));
            Assert.That(store.SavedCredentials.ApiSecret, Is.EqualTo("old-secret"));
            Assert.That(tokenCache.InvalidatedOrganizationId, Is.Null);
        });
    }

    [Test]
    public async Task UpsertBidconCredentials_ReturnsValidationErrors_WhenCommandIsIncomplete()
    {
        var store = new RecordingBidconApiCredentialsStore();
        var verifier = new RecordingBidconCredentialsVerifier(true);
        var handler = new UpsertBidconCredentialsCommandHandler(
            store,
            verifier,
            new RecordingBidconAccessTokenCache(),
            new RecordingBidconCredentialsAuditLog());

        var result = await handler.HandleAsync(
            new UpsertBidconCredentialsCommand(new OrganizationId(string.Empty), new UserId(string.Empty), " ", string.Empty),
            CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<InvalidUpsertBidconCredentialsRequest>());
            var invalidRequest = (InvalidUpsertBidconCredentialsRequest)result;
            Assert.That(invalidRequest.ValidationErrors.Select(error => error.Code), Is.EquivalentTo(new[]
            {
                "bidcon.credentials.organization.required",
                "bidcon.credentials.actor.required",
                "bidcon.credentials.apiKey.required",
                "bidcon.credentials.apiSecret.required"
            }));
            Assert.That(store.SavedCredentials, Is.Null);
            Assert.That(verifier.VerifiedCredentials, Is.Null);
        });
    }

    [Test]
    public async Task RemoveBidconCredentials_RemovesCredentialsAndInvalidatesToken_WhenOrganizationExists()
    {
        var organizationId = new OrganizationId("1c928774-3e22-4a94-9146-8784fb54d67a");
        var actorId = new UserId("admin-1");
        var store = new RecordingBidconApiCredentialsStore();
        await store.UpsertAsync(new BidconApiCredentials
        {
            OrganizationId = organizationId,
            ApiKey = "key-1",
            ApiSecret = "secret-1"
        });

        var tokenCache = new RecordingBidconAccessTokenCache();
        var handler = new RemoveBidconCredentialsCommandHandler(
            store,
            tokenCache,
            new RecordingBidconCredentialsAuditLog());

        var result = await handler.HandleAsync(
            new RemoveBidconCredentialsCommand(organizationId, actorId),
            CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<BidconCredentialsRemoved>());
            var removed = (BidconCredentialsRemoved)result;
            Assert.That(removed.OrganizationId, Is.EqualTo(organizationId));
            Assert.That(removed.IsConfigured, Is.False);
            Assert.That(store.SavedCredentials, Is.Null);
            Assert.That(tokenCache.InvalidatedOrganizationId, Is.EqualTo(organizationId));
        });
    }

    [Test]
    public async Task RemoveBidconCredentials_ReturnsValidationErrorAndDoesNotMutateCredentials_WhenActorIsMissing()
    {
        var organizationId = new OrganizationId("1c928774-3e22-4a94-9146-8784fb54d67a");
        var store = new RecordingBidconApiCredentialsStore();
        await store.UpsertAsync(new BidconApiCredentials
        {
            OrganizationId = organizationId,
            ApiKey = "key-1",
            ApiSecret = "secret-1"
        });
        var tokenCache = new RecordingBidconAccessTokenCache();
        var auditLog = new RecordingBidconCredentialsAuditLog();
        var handler = new RemoveBidconCredentialsCommandHandler(store, tokenCache, auditLog);

        var result = await handler.HandleAsync(
            new RemoveBidconCredentialsCommand(organizationId, new UserId(string.Empty)),
            CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<InvalidRemoveBidconCredentialsRequest>());
            var invalidRequest = (InvalidRemoveBidconCredentialsRequest)result;
            Assert.That(invalidRequest.ValidationErrors, Has.One.Matches<ValidationError>(error =>
                error.Code == "bidcon.credentials.actor.required" &&
                error.Field == nameof(RemoveBidconCredentialsCommand.ActorId)));
            Assert.That(store.SavedCredentials, Is.Not.Null);
            Assert.That(store.DeleteAttempts, Is.Zero);
            Assert.That(tokenCache.InvalidatedOrganizationId, Is.Null);
            Assert.That(auditLog.Entries, Is.Empty);
        });
    }

    [Test]
    public async Task BidconCredentialsAdministration_WritesSecretFreeAuditEntries_ForCredentialChanges()
    {
        var organizationId = new OrganizationId("1c928774-3e22-4a94-9146-8784fb54d67a");
        var actorId = new UserId("admin-1");
        var changedAt = new DateTimeOffset(2026, 07, 22, 10, 30, 00, TimeSpan.Zero);
        var store = new RecordingBidconApiCredentialsStore();
        var auditLog = new RecordingBidconCredentialsAuditLog(changedAt);
        var tokenCache = new RecordingBidconAccessTokenCache();

        var createHandler = new UpsertBidconCredentialsCommandHandler(
            store,
            new RecordingBidconCredentialsVerifier(true),
            tokenCache,
            auditLog);
        await createHandler.HandleAsync(
            new UpsertBidconCredentialsCommand(organizationId, actorId, "create-key", "create-secret"),
            CancellationToken.None);

        var replaceHandler = new UpsertBidconCredentialsCommandHandler(
            store,
            new RecordingBidconCredentialsVerifier(true),
            tokenCache,
            auditLog);
        await replaceHandler.HandleAsync(
            new UpsertBidconCredentialsCommand(organizationId, actorId, "replace-key", "replace-secret"),
            CancellationToken.None);

        var failureHandler = new UpsertBidconCredentialsCommandHandler(
            store,
            new RecordingBidconCredentialsVerifier(false),
            tokenCache,
            auditLog);
        await failureHandler.HandleAsync(
            new UpsertBidconCredentialsCommand(organizationId, actorId, "bad-key", "bad-secret"),
            CancellationToken.None);

        var removeHandler = new RemoveBidconCredentialsCommandHandler(store, tokenCache, auditLog);
        await removeHandler.HandleAsync(
            new RemoveBidconCredentialsCommand(organizationId, actorId),
            CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(auditLog.Entries.Select(entry => entry.Action), Is.EqualTo(new[]
            {
                BidconCredentialsAuditAction.Create,
                BidconCredentialsAuditAction.Replace,
                BidconCredentialsAuditAction.VerificationFailed,
                BidconCredentialsAuditAction.Remove
            }));
            Assert.That(auditLog.Entries.Select(entry => entry.Result), Is.EqualTo(new[]
            {
                BidconCredentialsAuditResult.Success,
                BidconCredentialsAuditResult.Success,
                BidconCredentialsAuditResult.Failure,
                BidconCredentialsAuditResult.Success
            }));
            Assert.That(auditLog.Entries, Has.All.Matches<BidconCredentialsAuditEntry>(entry =>
                entry.ActorId == actorId &&
                entry.OrganizationId == organizationId &&
                entry.OccurredAt == changedAt));
            var auditDetails = string.Join(Environment.NewLine, auditLog.Entries.Select(entry => entry.Details));
            Assert.That(auditDetails, Does.Not.Contain("create-secret"));
            Assert.That(auditDetails, Does.Not.Contain("replace-secret"));
            Assert.That(auditDetails, Does.Not.Contain("bad-secret"));
            Assert.That(auditDetails, Does.Not.Contain("create-key"));
            Assert.That(auditDetails, Does.Not.Contain("replace-key"));
            Assert.That(auditDetails, Does.Not.Contain("bad-key"));
        });
    }

    private sealed class RecordingBidconApiCredentialsStore : IBidconApiCredentialsStore
    {
        public BidconApiCredentials? SavedCredentials { get; private set; }

        public bool WasRead { get; private set; }

        public int DeleteAttempts { get; private set; }

        public Task<BidconApiCredentials?> GetAsync(OrganizationId orgId)
        {
            WasRead = true;
            return Task.FromResult(SavedCredentials?.OrganizationId == orgId ? SavedCredentials : null);
        }

        public Task UpsertAsync(BidconApiCredentials credentials)
        {
            SavedCredentials = credentials;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(OrganizationId orgId)
        {
            DeleteAttempts++;

            if (SavedCredentials?.OrganizationId == orgId)
            {
                SavedCredentials = null;
            }

            return Task.CompletedTask;
        }
    }

    private sealed class RecordingBidconCredentialsVerifier(bool isVerified) : IBidconCredentialsVerifier
    {
        public BidconApiCredentials? VerifiedCredentials { get; private set; }

        public Task<bool> VerifyAsync(BidconApiCredentials credentials, CancellationToken cancellationToken)
        {
            VerifiedCredentials = credentials;
            return Task.FromResult(isVerified);
        }
    }

    private sealed class RecordingBidconAccessTokenCache : IBidconAccessTokenCache
    {
        public OrganizationId? InvalidatedOrganizationId { get; private set; }

        public bool TryGet(OrganizationId organizationId, out string? accessToken)
        {
            accessToken = null;
            return false;
        }

        public void Set(OrganizationId organizationId, string accessToken, TimeSpan lifetime)
        {
        }

        public void Invalidate(OrganizationId organizationId)
        {
            InvalidatedOrganizationId = organizationId;
        }
    }

    private sealed class RecordingBidconCredentialsAuditLog(
        DateTimeOffset? occurredAt = null) : IBidconCredentialsAuditLog
    {
        public List<BidconCredentialsAuditEntry> Entries { get; } = [];

        public Task WriteAsync(
            BidconCredentialsAuditEntry entry,
            CancellationToken cancellationToken)
        {
            Entries.Add(entry with { OccurredAt = occurredAt ?? entry.OccurredAt });
            return Task.CompletedTask;
        }
    }
}

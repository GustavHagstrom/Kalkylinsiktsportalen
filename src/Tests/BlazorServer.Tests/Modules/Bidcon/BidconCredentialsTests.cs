using BlazorServer.Modules.Bidcon.Features.GetBidconCredentials;
using BlazorServer.Modules.Bidcon.Features.RemoveBidconCredentials;
using BlazorServer.Modules.Bidcon.Features.UpsertBidconCredentials;
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
            OrganizationId = organizationId.Value.ToString(),
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
        var store = new RecordingBidconApiCredentialsStore();
        var verifier = new RecordingBidconCredentialsVerifier(true);
        var tokenCache = new RecordingBidconAccessTokenCache();
        var handler = new UpsertBidconCredentialsCommandHandler(store, verifier, tokenCache);

        var result = await handler.HandleAsync(
            new UpsertBidconCredentialsCommand(organizationId, "key-1", "secret-1"),
            CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<BidconCredentialsSaved>());
            var saved = (BidconCredentialsSaved)result;
            Assert.That(saved.OrganizationId, Is.EqualTo(organizationId));
            Assert.That(saved.WasSaved, Is.True);
            Assert.That(store.SavedCredentials, Is.Not.Null);
            Assert.That(store.SavedCredentials!.OrganizationId, Is.EqualTo(organizationId.Value.ToString()));
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
        var store = new RecordingBidconApiCredentialsStore();
        await store.UpsertAsync(new BidconApiCredentials
        {
            OrganizationId = organizationId.Value.ToString(),
            ApiKey = "old-key",
            ApiSecret = "old-secret"
        });

        var tokenCache = new RecordingBidconAccessTokenCache();
        var handler = new UpsertBidconCredentialsCommandHandler(
            store,
            new RecordingBidconCredentialsVerifier(false),
            tokenCache);

        var result = await handler.HandleAsync(
            new UpsertBidconCredentialsCommand(organizationId, "new-key", "new-secret"),
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
        var handler = new UpsertBidconCredentialsCommandHandler(
            store,
            new RecordingBidconCredentialsVerifier(true),
            new RecordingBidconAccessTokenCache());

        var result = await handler.HandleAsync(
            new UpsertBidconCredentialsCommand(new OrganizationId(string.Empty), " ", string.Empty),
            CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<InvalidUpsertBidconCredentialsRequest>());
            var invalidRequest = (InvalidUpsertBidconCredentialsRequest)result;
            Assert.That(invalidRequest.ValidationErrors.Select(error => error.Code), Is.EquivalentTo(new[]
            {
                "bidcon.credentials.organization.required",
                "bidcon.credentials.apiKey.required",
                "bidcon.credentials.apiSecret.required"
            }));
            Assert.That(store.SavedCredentials, Is.Null);
        });
    }

    [Test]
    public async Task RemoveBidconCredentials_RemovesCredentialsAndInvalidatesToken_WhenOrganizationExists()
    {
        var organizationId = new OrganizationId("1c928774-3e22-4a94-9146-8784fb54d67a");
        var store = new RecordingBidconApiCredentialsStore();
        await store.UpsertAsync(new BidconApiCredentials
        {
            OrganizationId = organizationId.Value.ToString(),
            ApiKey = "key-1",
            ApiSecret = "secret-1"
        });

        var tokenCache = new RecordingBidconAccessTokenCache();
        var handler = new RemoveBidconCredentialsCommandHandler(store, tokenCache);

        var result = await handler.HandleAsync(
            new RemoveBidconCredentialsCommand(organizationId),
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

    private sealed class RecordingBidconApiCredentialsStore : IBidconApiCredentialsStore
    {
        public BidconApiCredentials? SavedCredentials { get; private set; }

        public bool WasRead { get; private set; }

        public Task<BidconApiCredentials?> GetAsync(string orgId)
        {
            WasRead = true;
            return Task.FromResult(SavedCredentials?.OrganizationId == orgId ? SavedCredentials : null);
        }

        public Task UpsertAsync(BidconApiCredentials credentials)
        {
            SavedCredentials = credentials;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(string orgId)
        {
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

        public void Invalidate(OrganizationId organizationId)
        {
            InvalidatedOrganizationId = organizationId;
        }
    }
}

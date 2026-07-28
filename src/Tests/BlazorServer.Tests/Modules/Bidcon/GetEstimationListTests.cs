using BlazorServer.Modules.Bidcon.Features;
using BlazorServer.Modules.Bidcon.Features.GetEstimationList;
using BlazorServer.Identity;

namespace BlazorServer.Tests.Modules.Bidcon;

[TestFixture]
public class GetEstimationListTests
{
    [Test]
    public async Task GetEstimationList_ReturnsMappedEstimations_WhenBidconReturnsList()
    {
        var organizationId = new OrganizationId("1c928774-3e22-4a94-9146-8784fb54d67a");
        var source = new StubBidconEstimationListSource(
        [
            new BidconEstimationListSourceItem(
                Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                "Beta",
                "Second"),
            new BidconEstimationListSourceItem(
                Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                "Alpha",
                "First")
        ]);
        var handler = new GetEstimationListQueryHandler(source);

        var result = await handler.HandleAsync(
            new GetEstimationListQuery(organizationId, "user@example.com"),
            CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<EstimationListLoaded>());
            var loaded = (EstimationListLoaded)result;
            Assert.That(loaded.WasLoaded, Is.True);
            Assert.That(loaded.Estimations, Has.Count.EqualTo(2));
            Assert.That(loaded.Estimations[0], Is.EqualTo(new GetEstimationListItem(
                "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
                "Beta",
                "Second")));
            Assert.That(loaded.Estimations[1], Is.EqualTo(new GetEstimationListItem(
                "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb",
                "Alpha",
                "First")));
            Assert.That(source.LastOrganizationId, Is.EqualTo(organizationId));
            Assert.That(source.LastUserEmail, Is.EqualTo("user@example.com"));
        });
    }

    [Test]
    public async Task GetEstimationList_ReturnsValidationErrors_WhenQueryIsMissingRequiredValues()
    {
        var source = new RecordingBidconEstimationListSource();
        var handler = new GetEstimationListQueryHandler(source);

        var result = await handler.HandleAsync(
            new GetEstimationListQuery(new OrganizationId(string.Empty), " "),
            CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<InvalidGetEstimationListRequest>());
            var invalidRequest = (InvalidGetEstimationListRequest)result;
            Assert.That(invalidRequest.ValidationErrors.Select(error => error.Code), Is.EquivalentTo(
            [
                "bidcon.estimationList.organization.required",
                "bidcon.estimationList.userEmail.required"
            ]));
            Assert.That(source.WasCalled, Is.False);
        });
    }

    [Test]
    public async Task GetEstimationList_ReturnsInvalidResult_WhenBidconClientFails()
    {
        var handler = new GetEstimationListQueryHandler(
            new FailingBidconEstimationListSource(
                new BidconFeatureSourceException(
                    "bidcon.credentials.missing",
                    nameof(GetEstimationListQuery.OrganizationId),
                    "Bidcon credentials are missing.")));

        var result = await handler.HandleAsync(
            new GetEstimationListQuery(
                new OrganizationId("1c928774-3e22-4a94-9146-8784fb54d67a"),
                "user@example.com"),
            CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<EstimationListBidconUnavailable>());
            var unavailable = (EstimationListBidconUnavailable)result;
            Assert.That(unavailable.ErrorCode, Is.EqualTo("bidcon.credentials.missing"));
            Assert.That(unavailable.ErrorField, Is.EqualTo(nameof(GetEstimationListQuery.OrganizationId)));
        });
    }

    [Test]
    public async Task GetEstimationList_ReturnsEmptyList_WhenBidconReturnsNoEstimations()
    {
        var handler = new GetEstimationListQueryHandler(new StubBidconEstimationListSource([]));

        var result = await handler.HandleAsync(
            new GetEstimationListQuery(
                new OrganizationId("1c928774-3e22-4a94-9146-8784fb54d67a"),
                "user@example.com"),
            CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<EstimationListLoaded>());
            var loaded = (EstimationListLoaded)result;
            Assert.That(loaded.WasLoaded, Is.True);
            Assert.That(loaded.Estimations, Is.Empty);
        });
    }

    private sealed class StubBidconEstimationListSource(IReadOnlyList<BidconEstimationListSourceItem> estimations) : IBidconEstimationListSource
    {
        public OrganizationId? LastOrganizationId { get; private set; }

        public string? LastUserEmail { get; private set; }

        public Task<IReadOnlyList<BidconEstimationListSourceItem>> GetEstimationListAsync(
            string userEmail,
            OrganizationId organizationId,
            CancellationToken cancellationToken)
        {
            LastUserEmail = userEmail;
            LastOrganizationId = organizationId;
            return Task.FromResult(estimations);
        }
    }

    private sealed class RecordingBidconEstimationListSource : IBidconEstimationListSource
    {
        public bool WasCalled { get; private set; }

        public Task<IReadOnlyList<BidconEstimationListSourceItem>> GetEstimationListAsync(
            string userEmail,
            OrganizationId organizationId,
            CancellationToken cancellationToken)
        {
            WasCalled = true;
            return Task.FromResult<IReadOnlyList<BidconEstimationListSourceItem>>([]);
        }
    }

    private sealed class FailingBidconEstimationListSource(Exception exception) : IBidconEstimationListSource
    {
        private readonly Exception _exception = exception;

        public Task<IReadOnlyList<BidconEstimationListSourceItem>> GetEstimationListAsync(
            string userEmail,
            OrganizationId organizationId,
            CancellationToken cancellationToken)
        {
            throw _exception;
        }
    }
}

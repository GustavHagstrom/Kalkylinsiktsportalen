using BlazorServer.Modules.Bidcon.Features.GetEstimationList;
using BlazorServer.Modules.Bidcon.Infrastructure.BidconApi.Clients;
using BlazorServer.Modules.Bidcon.Infrastructure.BidconApi.DTOs;
using BlazorServer.Kernel.Validation;
using BlazorServer.Modules.Bidcon.Public.Contracts;
using BlazorServer.Identity;

namespace BlazorServer.Tests.Modules.Bidcon;

[TestFixture]
public class GetEstimationListTests
{
    [Test]
    public async Task GetEstimationList_ReturnsMappedEstimations_WhenBidconReturnsList()
    {
        var organizationId = new OrganizationId("1c928774-3e22-4a94-9146-8784fb54d67a");
        var client = new StubBidconEstimationClient(
        [
            new EstimationDto
            {
                Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                Name = "Beta",
                Description = "Second"
            },
            new EstimationDto
            {
                Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                Name = "Alpha",
                Description = "First"
            }
        ]);
        var handler = new GetEstimationListQueryHandler(client);

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
            Assert.That(client.LastOrganizationId, Is.EqualTo(organizationId));
            Assert.That(client.LastUserEmail, Is.EqualTo("user@example.com"));
        });
    }

    [Test]
    public async Task GetEstimationList_ReturnsValidationErrors_WhenQueryIsMissingRequiredValues()
    {
        var client = new RecordingBidconEstimationClient();
        var handler = new GetEstimationListQueryHandler(client);

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
            Assert.That(client.WasCalled, Is.False);
        });
    }

    [Test]
    public async Task GetEstimationList_ReturnsInvalidResult_WhenBidconClientFails()
    {
        var handler = new GetEstimationListQueryHandler(
            new FailingBidconEstimationClient(
                new BidconEstimationClientException(
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
        var handler = new GetEstimationListQueryHandler(new StubBidconEstimationClient([]));

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

    private sealed class StubBidconEstimationClient(IReadOnlyList<EstimationDto> estimations) : BidconEstimationClient
    {
        private readonly IReadOnlyList<EstimationDto> _estimations = estimations;

        public OrganizationId? LastOrganizationId { get; private set; }

        public string? LastUserEmail { get; private set; }

        public override Task<IReadOnlyList<EstimationDto>> GetEstimationListAsync(
            string userEmail,
            OrganizationId organizationId,
            CancellationToken cancellationToken)
        {
            LastUserEmail = userEmail;
            LastOrganizationId = organizationId;
            return Task.FromResult(_estimations);
        }
    }

    private sealed class RecordingBidconEstimationClient : BidconEstimationClient
    {
        public bool WasCalled { get; private set; }

        public override Task<IReadOnlyList<EstimationDto>> GetEstimationListAsync(
            string userEmail,
            OrganizationId organizationId,
            CancellationToken cancellationToken)
        {
            WasCalled = true;
            return Task.FromResult<IReadOnlyList<EstimationDto>>([]);
        }
    }

    private sealed class FailingBidconEstimationClient(Exception exception) : BidconEstimationClient
    {
        private readonly Exception _exception = exception;

        public override Task<IReadOnlyList<EstimationDto>> GetEstimationListAsync(
            string userEmail,
            OrganizationId organizationId,
            CancellationToken cancellationToken)
        {
            throw _exception;
        }
    }
}

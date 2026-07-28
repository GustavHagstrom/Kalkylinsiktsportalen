using BlazorServer.Identity;
using BlazorServer.Modules.Bidcon.Domain;
using BlazorServer.Modules.Bidcon.Public.Contracts;

namespace BlazorServer.Modules.Bidcon.Features.ImportEstimationSnapshot;

internal interface IBidconEstimationSnapshotSource
{
    Task<BidconEstimationSnapshotSourceDetails?> GetSnapshotDetailsAsync(
        BidconEstimationId bidconEstimationId,
        OrganizationId organizationId,
        CancellationToken cancellationToken);
}

internal sealed record BidconEstimationSnapshotSourceDetails(
    DateTime SourceEditDate,
    EstimationSnapshotPayloadV1 Payload);

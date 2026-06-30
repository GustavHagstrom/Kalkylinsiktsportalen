using BlazorServer.Modules.Bidcon.Contracts;

namespace BlazorServer.Modules.Bidcon.Features.ImportEstimationSnapshot;

public sealed record ImportEstimationSnapshotResult(
    EstimationSnapshotId SnapshotId,
    BidconEstimationId BidconEstimationId,
    DateTime SourceEditDate,
    DateTimeOffset ImportedAt);

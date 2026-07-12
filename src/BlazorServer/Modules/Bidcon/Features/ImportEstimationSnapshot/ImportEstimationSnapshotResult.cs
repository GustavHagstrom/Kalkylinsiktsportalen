using BlazorServer.Kernel.Validation;
using BlazorServer.Modules.Bidcon.Public.Contracts;

namespace BlazorServer.Modules.Bidcon.Features.ImportEstimationSnapshot;

public abstract record ImportEstimationSnapshotResult;

public sealed record EstimationSnapshotImported(
    EstimationSnapshotId SnapshotId,
    BidconEstimationId BidconEstimationId,
    DateTime SourceEditDate,
    DateTimeOffset ImportedAt) : ImportEstimationSnapshotResult
{
    public bool WasImported => true;
}

public sealed record InvalidImportEstimationSnapshotRequest(
    IReadOnlyList<ValidationError> ValidationErrors) : ImportEstimationSnapshotResult;

public sealed record ImportEstimationSnapshotBidconUnavailable(
    string ErrorCode,
    string ErrorField,
    string ErrorMessage) : ImportEstimationSnapshotResult;

public sealed record ImportEstimationSnapshotNotFound(
    string ErrorCode,
    string ErrorField,
    string ErrorMessage) : ImportEstimationSnapshotResult;

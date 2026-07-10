using BlazorServer.Modules.Bidcon.Public.Contracts;
using BlazorServer.Kernel.Validation;

namespace BlazorServer.Modules.Bidcon.Features.ImportEstimationSnapshot;

public sealed record ImportEstimationSnapshotResult(
    EstimationSnapshotId? SnapshotId,
    BidconEstimationId? BidconEstimationId,
    DateTime? SourceEditDate,
    DateTimeOffset? ImportedAt,
    IReadOnlyList<ValidationError> Errors) : IValidationResult
{
    public bool IsValid => Errors.Count == 0;

    public bool IsSuccess => IsValid;

    public static ImportEstimationSnapshotResult Success(
        EstimationSnapshotId snapshotId,
        BidconEstimationId bidconEstimationId,
        DateTime sourceEditDate,
        DateTimeOffset importedAt)
        => new(snapshotId, bidconEstimationId, sourceEditDate, importedAt, Array.Empty<ValidationError>());

    public static ImportEstimationSnapshotResult Invalid(params ValidationError[] errors)
        => new(null, null, null, null, errors);
}

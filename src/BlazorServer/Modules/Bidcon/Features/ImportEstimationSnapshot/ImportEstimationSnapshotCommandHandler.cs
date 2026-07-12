using BlazorServer.Modules.Bidcon.Domain;
using BlazorServer.Modules.Bidcon.Infrastructure.BidconApi.Clients;
using BlazorServer.Modules.Bidcon.Infrastructure.BidconApi.DTOs;
using BlazorServer.Modules.Bidcon.Infrastructure.BidconApi.Mapping;
using BlazorServer.Modules.Bidcon.Infrastructure.Persistence;
using BlazorServer.Modules.Bidcon.Public.Contracts;
using BlazorServer.Kernel.CQRS;
using BlazorServer.Kernel.Validation;

namespace BlazorServer.Modules.Bidcon.Features.ImportEstimationSnapshot;

internal sealed class ImportEstimationSnapshotCommandHandler(
    BidconEstimationClient estimationDetailsClient,
    IEstimationSnapshotStore estimationSnapshotStore,
    ImportEstimationSnapshotCommandValidator? validator = null)
        : ICommandHandler<ImportEstimationSnapshotCommand, ImportEstimationSnapshotResult>
{
    private readonly BidconEstimationClient _estimationDetailsClient = estimationDetailsClient;
    private readonly IEstimationSnapshotStore _estimationSnapshotStore = estimationSnapshotStore;
    private readonly ImportEstimationSnapshotCommandValidator _validator = validator ?? new ImportEstimationSnapshotCommandValidator();

    public async Task<ImportEstimationSnapshotResult> HandleAsync(
        ImportEstimationSnapshotCommand command,
        CancellationToken cancellationToken)
    {
        var errors = _validator.Validate(command);
        if (errors.Count > 0)
        {
            return new InvalidImportEstimationSnapshotRequest(errors);
        }

        EstimationDetailsDto? estimationDetails;

        try
        {
            estimationDetails = await _estimationDetailsClient.GetEstimationDetailsAsync(
                command.BidconEstimationId,
                command.OrganizationId,
                cancellationToken);
        }
        catch (BidconEstimationClientException exception)
        {
            return new ImportEstimationSnapshotBidconUnavailable(
                exception.Code,
                exception.Field,
                exception.Message);
        }

        if (estimationDetails is null)
        {
            return new ImportEstimationSnapshotNotFound(
                "bidcon.estimation.notFound",
                nameof(command.BidconEstimationId),
                "Bidcon estimation was not found.");
        }

        var importedAt = DateTimeOffset.UtcNow;
        var snapshot = EstimationSnapshot.Create(
            new EstimationSnapshotId(Guid.NewGuid()),
            command.OrganizationId,
            command.BidconEstimationId,
            estimationDetails.EditDate,
            importedAt,
            estimationDetails.ToSnapshotPayload());

        await _estimationSnapshotStore.AddAsync(snapshot, cancellationToken);

        return new EstimationSnapshotImported(
            snapshot.Id,
            snapshot.BidconEstimationId,
            snapshot.SourceEditDate,
            snapshot.ImportedAt);
    }

}

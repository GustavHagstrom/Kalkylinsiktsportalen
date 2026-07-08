using BlazorServer.Modules.Bidcon.Domain;
using BlazorServer.Modules.Bidcon.Infrastructure.BidconApi;
using BlazorServer.Modules.Bidcon.Infrastructure.BidconApi.ExternalDTOs;
using BlazorServer.Modules.Bidcon.Infrastructure.Persistence;
using BlazorServer.Modules.Bidcon.Public.Contracts;
using BlazorServer.Shared.Kernel.CQRS;
using BlazorServer.Shared.Kernel.Validation;

namespace BlazorServer.Modules.Bidcon.Features.ImportEstimationSnapshot;

public sealed class ImportEstimationSnapshotCommandHandler(
    BidconEstimationClient estimationDetailsClient,
    IEstimationSnapshotStore estimationSnapshotStore,
    IValidator<ImportEstimationSnapshotCommand>? validator = null)
        : ICommandHandler<ImportEstimationSnapshotCommand, ImportEstimationSnapshotResult>
{
    private readonly BidconEstimationClient _estimationDetailsClient = estimationDetailsClient;
    private readonly IEstimationSnapshotStore _estimationSnapshotStore = estimationSnapshotStore;
    private readonly IValidator<ImportEstimationSnapshotCommand> _validator = validator ?? new ImportEstimationSnapshotCommandValidator();

    public async Task<CommandResult<ImportEstimationSnapshotResult>> HandleAsync(
        ImportEstimationSnapshotCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = _validator.Validate(command);

        if (!validationResult.IsValid)
        {
            return CommandResult<ImportEstimationSnapshotResult>.Invalid([.. validationResult.Errors]);
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
            return CommandResult<ImportEstimationSnapshotResult>.Invalid(
                new ValidationError(exception.Code, exception.Field, exception.Message));
        }

        if (estimationDetails is null)
        {
            return CommandResult<ImportEstimationSnapshotResult>.Invalid(
                new ValidationError(
                    "bidcon.estimation.notFound",
                    nameof(command.BidconEstimationId),
                    "Bidcon estimation was not found."));
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

        return CommandResult<ImportEstimationSnapshotResult>.Success(
            new ImportEstimationSnapshotResult(
                snapshot.Id,
                snapshot.BidconEstimationId,
                snapshot.SourceEditDate,
                snapshot.ImportedAt));
    }

}

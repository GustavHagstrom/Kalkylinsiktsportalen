using BlazorServer.Modules.Bidcon.Contracts;
using BlazorServer.Modules.Bidcon.Domain;
using BlazorServer.Modules.Bidcon.Features.ImportEstimationSnapshot.ExternalDTOs;
using BlazorServer.Modules.Bidcon.Infrastructure.Persistence;
using BlazorServer.Shared.Kernel.CQRS;
using BlazorServer.Shared.Kernel.Validation;

namespace BlazorServer.Modules.Bidcon.Features.ImportEstimationSnapshot;

public sealed class ImportEstimationSnapshotCommandHandler
    : ICommandHandler<ImportEstimationSnapshotCommand, ImportEstimationSnapshotResult>
{
    private readonly BidconEstimationDetailsClient estimationDetailsClient;
    private readonly IEstimationSnapshotStore estimationSnapshotStore;
    private readonly IValidator<ImportEstimationSnapshotCommand> validator;

    public ImportEstimationSnapshotCommandHandler(
        BidconEstimationDetailsClient estimationDetailsClient,
        IEstimationSnapshotStore estimationSnapshotStore,
        IValidator<ImportEstimationSnapshotCommand>? validator = null)
    {
        this.estimationDetailsClient = estimationDetailsClient;
        this.estimationSnapshotStore = estimationSnapshotStore;
        this.validator = validator ?? new ImportEstimationSnapshotCommandValidator();
    }

    public async Task<CommandResult<ImportEstimationSnapshotResult>> HandleAsync(
        ImportEstimationSnapshotCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = validator.Validate(command);

        if (!validationResult.IsValid)
        {
            return CommandResult<ImportEstimationSnapshotResult>.Invalid(validationResult.Errors.ToArray());
        }

        EstimationDetailsDto? estimationDetails;

        try
        {
            estimationDetails = await estimationDetailsClient.GetEstimationDetailsAsync(
                command.BidconEstimationId,
                command.OrganizationId,
                cancellationToken);
        }
        catch (BidconEstimationDetailsClientException exception)
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

        await estimationSnapshotStore.AddAsync(snapshot, cancellationToken);

        return CommandResult<ImportEstimationSnapshotResult>.Success(
            new ImportEstimationSnapshotResult(
                snapshot.Id,
                snapshot.BidconEstimationId,
                snapshot.SourceEditDate,
                snapshot.ImportedAt));
    }

}

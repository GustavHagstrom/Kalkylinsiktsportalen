using BlazorServer.Shared.Kernel.Validation;

namespace BlazorServer.Modules.Bidcon.Features.ImportEstimationSnapshot;

public sealed class ImportEstimationSnapshotCommandValidator : IValidator<ImportEstimationSnapshotCommand>
{
    public ValidationResult Validate(ImportEstimationSnapshotCommand instance)
    {
        var errors = new List<ValidationError>();

        if (instance.OrganizationId.IsEmpty)
        {
            errors.Add(new ValidationError(
                "bidcon.import.organization.required",
                nameof(instance.OrganizationId),
                "Organization id is required."));
        }

        if (instance.BidconEstimationId.IsEmpty)
        {
            errors.Add(new ValidationError(
                "bidcon.import.estimation.required",
                nameof(instance.BidconEstimationId),
                "Bidcon estimation id is required."));
        }

        return errors.Count == 0
            ? ValidationResult.Success()
            : ValidationResult.Failure(errors.ToArray());
    }
}

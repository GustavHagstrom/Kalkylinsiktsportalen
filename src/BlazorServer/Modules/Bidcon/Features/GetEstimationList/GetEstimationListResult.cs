using BlazorServer.Kernel.Validation;

namespace BlazorServer.Modules.Bidcon.Features.GetEstimationList;

public sealed record GetEstimationListResult(
    IReadOnlyList<GetEstimationListItem> Estimations,
    IReadOnlyList<ValidationError> Errors) : IValidationResult
{
    public bool IsValid => Errors.Count == 0;

    public bool IsSuccess => IsValid;

    public static GetEstimationListResult Success(IReadOnlyList<GetEstimationListItem> estimations)
        => new(estimations, Array.Empty<ValidationError>());

    public static GetEstimationListResult Invalid(params ValidationError[] errors)
        => new(Array.Empty<GetEstimationListItem>(), errors);
}

public sealed record GetEstimationListItem(
    string Id,
    string Name,
    string? Description);

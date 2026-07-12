using BlazorServer.Kernel.Validation;

namespace BlazorServer.Modules.Bidcon.Features.GetEstimationList;

public abstract record GetEstimationListResult;

public sealed record EstimationListLoaded(
    IReadOnlyList<GetEstimationListItem> Estimations) : GetEstimationListResult
{
    public bool WasLoaded => true;
}

public sealed record InvalidGetEstimationListRequest(
    IReadOnlyList<ValidationError> ValidationErrors) : GetEstimationListResult;

public sealed record EstimationListBidconUnavailable(
    string ErrorCode,
    string ErrorField,
    string ErrorMessage) : GetEstimationListResult;

public sealed record GetEstimationListItem(
    string Id,
    string Name,
    string? Description);

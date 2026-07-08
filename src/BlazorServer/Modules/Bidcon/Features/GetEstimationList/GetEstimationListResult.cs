namespace BlazorServer.Modules.Bidcon.Features.GetEstimationList;

public sealed record GetEstimationListResult(
    IReadOnlyList<GetEstimationListItem> Estimations);

public sealed record GetEstimationListItem(
    string Id,
    string Name,
    string? Description);

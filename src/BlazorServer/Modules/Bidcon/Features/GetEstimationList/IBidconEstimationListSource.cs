using BlazorServer.Identity;

namespace BlazorServer.Modules.Bidcon.Features.GetEstimationList;

internal interface IBidconEstimationListSource
{
    Task<IReadOnlyList<BidconEstimationListSourceItem>> GetEstimationListAsync(
        string userEmail,
        OrganizationId organizationId,
        CancellationToken cancellationToken);
}

internal sealed record BidconEstimationListSourceItem(
    Guid Id,
    string Name,
    string? Description);

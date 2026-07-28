using BlazorServer.Identity;
using BlazorServer.Modules.Bidcon.Features;
using BlazorServer.Modules.Bidcon.Features.GetEstimationList;
using BlazorServer.Modules.Bidcon.Infrastructure.BidconApi.Clients;

namespace BlazorServer.Modules.Bidcon.Infrastructure.BidconApi;

internal sealed class BidconEstimationListSource(BidconEstimationClient estimationClient)
    : IBidconEstimationListSource
{
    public async Task<IReadOnlyList<BidconEstimationListSourceItem>> GetEstimationListAsync(
        string userEmail,
        OrganizationId organizationId,
        CancellationToken cancellationToken)
    {
        try
        {
            var estimations = await estimationClient.GetEstimationListAsync(
                userEmail,
                organizationId,
                cancellationToken);

            return estimations
                .Select(estimation => new BidconEstimationListSourceItem(
                    estimation.Id,
                    estimation.Name,
                    estimation.Description))
                .ToList();
        }
        catch (BidconEstimationClientException exception)
        {
            throw new BidconFeatureSourceException(
                exception.Code,
                exception.Field,
                exception.Message);
        }
    }
}

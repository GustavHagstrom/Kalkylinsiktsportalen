using BlazorServer.Identity;
using BlazorServer.Modules.Bidcon.Features;
using BlazorServer.Modules.Bidcon.Features.ImportEstimationSnapshot;
using BlazorServer.Modules.Bidcon.Infrastructure.BidconApi.Clients;
using BlazorServer.Modules.Bidcon.Infrastructure.BidconApi.Mapping;
using BlazorServer.Modules.Bidcon.Public.Contracts;

namespace BlazorServer.Modules.Bidcon.Infrastructure.BidconApi;

internal sealed class BidconEstimationSnapshotSource(BidconEstimationClient estimationClient)
    : IBidconEstimationSnapshotSource
{
    public async Task<BidconEstimationSnapshotSourceDetails?> GetSnapshotDetailsAsync(
        BidconEstimationId bidconEstimationId,
        OrganizationId organizationId,
        CancellationToken cancellationToken)
    {
        try
        {
            var details = await estimationClient.GetEstimationDetailsAsync(
                bidconEstimationId,
                organizationId,
                cancellationToken);

            return details is null
                ? null
                : new BidconEstimationSnapshotSourceDetails(
                    details.EditDate,
                    details.ToSnapshotPayload());
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

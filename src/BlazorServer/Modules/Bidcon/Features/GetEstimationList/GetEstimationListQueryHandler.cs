using BlazorServer.Modules.Bidcon.Infrastructure.BidconApi.Clients;
using BlazorServer.Kernel.CQRS;

namespace BlazorServer.Modules.Bidcon.Features.GetEstimationList;

internal sealed class GetEstimationListQueryHandler(
    BidconEstimationClient estimationClient,
    GetEstimationListQueryValidator? validator = null)
        : IQueryHandler<GetEstimationListQuery, GetEstimationListResult>
{
    private readonly BidconEstimationClient _estimationClient = estimationClient;
    private readonly GetEstimationListQueryValidator _validator = validator ?? new GetEstimationListQueryValidator();

    public async Task<GetEstimationListResult> HandleAsync(
        GetEstimationListQuery query,
        CancellationToken cancellationToken)
    {
        var errors = _validator.Validate(query);
        if (errors.Count > 0)
        {
            return new InvalidGetEstimationListRequest(errors);
        }

        try
        {
            var estimations = await _estimationClient.GetEstimationListAsync(
                query.UserEmail,
                query.OrganizationId,
                cancellationToken);

            return new EstimationListLoaded(
                estimations
                    .Select(estimation => new GetEstimationListItem(
                        estimation.Id.ToString(),
                        estimation.Name,
                        estimation.Description))
                    .ToList());
        }
        catch (BidconEstimationClientException exception)
        {
            return new EstimationListBidconUnavailable(exception.Code, exception.Field, exception.Message);
        }
    }
}

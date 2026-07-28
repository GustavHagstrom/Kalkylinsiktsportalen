using BlazorServer.Modules.Bidcon.Features;
using BlazorServer.Kernel.CQRS;

namespace BlazorServer.Modules.Bidcon.Features.GetEstimationList;

internal sealed class GetEstimationListQueryHandler(
    IBidconEstimationListSource estimationListSource,
    GetEstimationListQueryValidator? validator = null)
        : IQueryHandler<GetEstimationListQuery, GetEstimationListResult>
{
    private readonly IBidconEstimationListSource _estimationListSource = estimationListSource;
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
            var estimations = await _estimationListSource.GetEstimationListAsync(
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
        catch (BidconFeatureSourceException exception)
        {
            return new EstimationListBidconUnavailable(exception.Code, exception.Field, exception.Message);
        }
    }
}

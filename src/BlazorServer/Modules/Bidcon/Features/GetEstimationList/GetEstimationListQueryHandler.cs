using BlazorServer.Modules.Bidcon.Infrastructure.BidconApi;
using BlazorServer.Shared.Kernel.CQRS;
using BlazorServer.Shared.Kernel.Validation;

namespace BlazorServer.Modules.Bidcon.Features.GetEstimationList;

public sealed class GetEstimationListQueryHandler(
    BidconEstimationClient estimationClient,
    IValidator<GetEstimationListQuery>? validator = null)
        : IQueryHandler<GetEstimationListQuery, GetEstimationListResult>
{
    private readonly BidconEstimationClient _estimationClient = estimationClient;
    private readonly IValidator<GetEstimationListQuery> _validator = validator ?? new GetEstimationListQueryValidator();

    public async Task<QueryResult<GetEstimationListResult>> HandleAsync(
        GetEstimationListQuery query,
        CancellationToken cancellationToken)
    {
        var validationResult = _validator.Validate(query);

        if (!validationResult.IsValid)
        {
            return QueryResult<GetEstimationListResult>.Invalid([.. validationResult.Errors]);
        }

        try
        {
            var estimations = await _estimationClient.GetEstimationListAsync(
                query.UserEmail,
                query.OrganizationId,
                cancellationToken);

            return QueryResult<GetEstimationListResult>.Success(
                new GetEstimationListResult(
                    estimations
                        .Select(estimation => new GetEstimationListItem(
                            estimation.Id.ToString(),
                            estimation.Name,
                            estimation.Description))
                        .ToList()));
        }
        catch (BidconEstimationClientException exception)
        {
            return QueryResult<GetEstimationListResult>.Invalid(
                new ValidationError(exception.Code, exception.Field, exception.Message));
        }
    }
}

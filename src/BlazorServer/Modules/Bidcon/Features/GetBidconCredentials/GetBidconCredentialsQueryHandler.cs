using BlazorServer.Modules.Bidcon.Infrastructure.Persistence;
using BlazorServer.Shared.Kernel.CQRS;
using BlazorServer.Shared.Kernel.Validation;

namespace BlazorServer.Modules.Bidcon.Features.GetBidconCredentials;

public sealed class GetBidconCredentialsQueryHandler
    : IQueryHandler<GetBidconCredentialsQuery, GetBidconCredentialsResult>
{
    private readonly IBidconApiCredentialsStore credentialsStore;
    private readonly IValidator<GetBidconCredentialsQuery> validator;

    public GetBidconCredentialsQueryHandler(
        IBidconApiCredentialsStore credentialsStore,
        IValidator<GetBidconCredentialsQuery>? validator = null)
    {
        this.credentialsStore = credentialsStore;
        this.validator = validator ?? new GetBidconCredentialsQueryValidator();
    }

    public async Task<QueryResult<GetBidconCredentialsResult>> HandleAsync(
        GetBidconCredentialsQuery query,
        CancellationToken cancellationToken)
    {
        var validationResult = validator.Validate(query);

        if (!validationResult.IsValid)
        {
            return QueryResult<GetBidconCredentialsResult>.Invalid(validationResult.Errors.ToArray());
        }

        var credentials = await credentialsStore.GetAsync(query.OrganizationId.Value.ToString());

        if (credentials is null ||
            string.IsNullOrWhiteSpace(credentials.ApiKey) ||
            string.IsNullOrWhiteSpace(credentials.ApiSecret))
        {
            return QueryResult<GetBidconCredentialsResult>.Success(
                new GetBidconCredentialsResult(query.OrganizationId, string.Empty, string.Empty, false));
        }

        return QueryResult<GetBidconCredentialsResult>.Success(
            new GetBidconCredentialsResult(
                query.OrganizationId,
                MaskCredential(credentials.ApiKey),
                MaskCredential(credentials.ApiSecret),
                true));
    }

    private static string MaskCredential(string value)
    {
        const int visibleCharacters = 4;

        if (value.Length <= visibleCharacters)
        {
            return "****";
        }

        return $"****{value[^visibleCharacters..]}";
    }
}

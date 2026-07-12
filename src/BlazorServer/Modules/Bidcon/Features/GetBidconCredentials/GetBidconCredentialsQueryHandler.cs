using BlazorServer.Modules.Bidcon.Infrastructure.Persistence;
using BlazorServer.Kernel.CQRS;
using BlazorServer.Kernel.Validation;

namespace BlazorServer.Modules.Bidcon.Features.GetBidconCredentials;

public sealed class GetBidconCredentialsQueryHandler
    : IQueryHandler<GetBidconCredentialsQuery, GetBidconCredentialsResult>
{
    private readonly IBidconApiCredentialsStore credentialsStore;
    private readonly GetBidconCredentialsQueryValidator validator;

    public GetBidconCredentialsQueryHandler(
        IBidconApiCredentialsStore credentialsStore,
        GetBidconCredentialsQueryValidator? validator = null)
    {
        this.credentialsStore = credentialsStore;
        this.validator = validator ?? new GetBidconCredentialsQueryValidator();
    }

    public async Task<GetBidconCredentialsResult> HandleAsync(
        GetBidconCredentialsQuery query,
        CancellationToken cancellationToken)
    {
        var errors = validator.Validate(query);
        if (errors.Count > 0)
        {
            return new InvalidGetBidconCredentialsRequest(errors);
        }

        var credentials = await credentialsStore.GetAsync(query.OrganizationId.Value.ToString());

        if (credentials is null ||
            string.IsNullOrWhiteSpace(credentials.ApiKey) ||
            string.IsNullOrWhiteSpace(credentials.ApiSecret))
        {
            return new BidconCredentialsNotConfigured(query.OrganizationId);
        }

        return new BidconCredentialsFound(
            query.OrganizationId,
            MaskCredential(credentials.ApiKey),
            MaskCredential(credentials.ApiSecret));
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

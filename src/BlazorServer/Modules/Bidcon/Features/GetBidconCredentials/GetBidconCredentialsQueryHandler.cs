using BlazorServer.Modules.Bidcon.Infrastructure.Persistence;
using BlazorServer.Kernel.CQRS;

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
            return GetBidconCredentialsResult.Invalid([.. errors]);
        }

        var credentials = await credentialsStore.GetAsync(query.OrganizationId.Value.ToString());

        if (credentials is null ||
            string.IsNullOrWhiteSpace(credentials.ApiKey) ||
            string.IsNullOrWhiteSpace(credentials.ApiSecret))
        {
            return GetBidconCredentialsResult.Success(query.OrganizationId, string.Empty, string.Empty, false);
        }

        return GetBidconCredentialsResult.Success(
            query.OrganizationId,
            MaskCredential(credentials.ApiKey),
            MaskCredential(credentials.ApiSecret),
            true);
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

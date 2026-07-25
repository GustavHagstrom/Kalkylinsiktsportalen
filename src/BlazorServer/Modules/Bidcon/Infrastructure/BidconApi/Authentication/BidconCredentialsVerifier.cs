using BlazorServer.Modules.Bidcon.Features.UpsertBidconCredentials;
using BlazorServer.Modules.Bidcon.Infrastructure.BidconApi.DTOs;
using BlazorServer.Modules.Bidcon.Infrastructure.Persistence;

namespace BlazorServer.Modules.Bidcon.Infrastructure.BidconApi.Authentication;

internal sealed class BidconCredentialsVerifier(IHttpClientFactory httpClientFactory) : IBidconCredentialsVerifier
{
    public async Task<bool> VerifyAsync(BidconApiCredentials credentials, CancellationToken cancellationToken)
    {
        var client = httpClientFactory.CreateClient(BidconHttpClientNames.ApiClient);
        var response = await client.PostAsJsonAsync(
            "Authentication/Authenticate",
            new ApiKeyAuthenticateDto
            {
                ApiKey = credentials.ApiKey,
                Secret = credentials.ApiSecret
            },
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        var token = await response.Content.ReadFromJsonAsync<ApiKeyAuthenticateResultDto>(cancellationToken);

        return token is not null && !string.IsNullOrWhiteSpace(token.AccessToken);
    }
}

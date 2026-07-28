using System.Net;
using System.Net.Http.Headers;
using BlazorServer.Identity;
using BlazorServer.Modules.Bidcon.AccessTokens;
using BlazorServer.Modules.Bidcon.Features;
using BlazorServer.Modules.Bidcon.Features.GetEstimationList;
using BlazorServer.Modules.Bidcon.Features.ImportEstimationSnapshot;
using BlazorServer.Modules.Bidcon.Infrastructure.BidconApi.DTOs;
using BlazorServer.Modules.Bidcon.Infrastructure.BidconApi.Mapping;
using BlazorServer.Modules.Bidcon.Infrastructure.Persistence;
using BlazorServer.Modules.Bidcon.Infrastructure.BidconApi.Authentication;
using BlazorServer.Modules.Bidcon.Public.Contracts;

namespace BlazorServer.Modules.Bidcon.Infrastructure.BidconApi.Clients;

internal class BidconEstimationClient : IBidconEstimationListSource, IBidconEstimationSnapshotSource
{
    private readonly IHttpClientFactory? httpClientFactory;
    private readonly IBidconAccessTokenCache? accessTokenCache;
    private readonly IBidconApiCredentialsStore? credentialsStore;

    protected BidconEstimationClient()
    {
    }

    public BidconEstimationClient(
        IHttpClientFactory httpClientFactory,
        IBidconAccessTokenCache accessTokenCache,
        IBidconApiCredentialsStore credentialsStore)
    {
        this.httpClientFactory = httpClientFactory;
        this.accessTokenCache = accessTokenCache;
        this.credentialsStore = credentialsStore;
    }

    public async virtual Task<IReadOnlyList<BidconEstimationListSourceItem>> GetEstimationListAsync(
        string userEmail,
        OrganizationId organizationId,
        CancellationToken cancellationToken)
    {
        try
        {
            var estimations = await GetEstimationDtosAsync(
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
                exception.Message,
                exception);
        }
    }

    public async virtual Task<BidconEstimationSnapshotSourceDetails?> GetSnapshotDetailsAsync(
        BidconEstimationId bidconEstimationId,
        OrganizationId organizationId,
        CancellationToken cancellationToken)
    {
        try
        {
            var details = await GetEstimationDetailsDtoAsync(
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
                exception.Message,
                exception);
        }
    }

    private async Task<IReadOnlyList<EstimationDto>> GetEstimationDtosAsync(
        string userEmail,
        OrganizationId organizationId,
        CancellationToken cancellationToken)
    {
        try
        {
            var client = await GetAuthenticatedClientAsync(organizationId, cancellationToken);
            var response = await client.GetAsync($"Estimations/{userEmail}", cancellationToken);

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return [];
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<EstimationDto>>(cancellationToken) ?? [];
        }
        catch (BidconEstimationClientException)
        {
            throw;
        }
        catch (HttpRequestException exception)
        {
            throw new BidconEstimationClientException(
                "bidcon.estimationClient.failed",
                nameof(GetEstimationListQuery.UserEmail),
                exception.Message);
        }
    }

    private async Task<EstimationDetailsDto?> GetEstimationDetailsDtoAsync(
        BidconEstimationId bidconEstimationId,
        OrganizationId organizationId,
        CancellationToken cancellationToken)
    {
        try
        {
            var client = await GetAuthenticatedClientAsync(organizationId, cancellationToken);
            var uriResponse = await client.GetAsync($"EstimationUri/{bidconEstimationId.Value}", cancellationToken);

            if (uriResponse.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            uriResponse.EnsureSuccessStatusCode();
            var hrefToEstimation = await uriResponse.Content.ReadAsStringAsync(cancellationToken);

            client.DefaultRequestHeaders.Authorization = null;
            var estimationResponse = await client.GetAsync(hrefToEstimation, cancellationToken);

            if (estimationResponse.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            estimationResponse.EnsureSuccessStatusCode();
            return await estimationResponse.Content.ReadFromJsonAsync<EstimationDetailsDto>(cancellationToken);
        }
        catch (BidconEstimationClientException)
        {
            throw;
        }
        catch (HttpRequestException exception)
        {
            throw new BidconEstimationClientException(
                "bidcon.estimationClient.failed",
                nameof(BidconEstimationId),
                exception.Message);
        }
    }

    private async Task<HttpClient> GetAuthenticatedClientAsync(
        OrganizationId organizationId,
        CancellationToken cancellationToken)
    {
        var client = httpClientFactory!.CreateClient(BidconHttpClientNames.ApiClient);

        if (accessTokenCache!.TryGet(organizationId, out var cachedAccessToken))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", cachedAccessToken);
            return client;
        }

        var credentials = await credentialsStore!.GetAsync(organizationId);

        if (credentials is null)
        {
            throw new BidconEstimationClientException(
                "bidcon.credentials.missing",
                nameof(OrganizationId),
                "Bidcon credentials are missing.");
        }

        var tokenResponse = await client.PostAsJsonAsync(
            "Authentication/Authenticate",
            new ApiKeyAuthenticateDto
            {
                ApiKey = credentials.ApiKey,
                Secret = credentials.ApiSecret
            },
            cancellationToken);

        tokenResponse.EnsureSuccessStatusCode();
        var token = await tokenResponse.Content.ReadFromJsonAsync<ApiKeyAuthenticateResultDto>(cancellationToken);

        if (token is null || string.IsNullOrWhiteSpace(token.AccessToken))
        {
            throw new BidconEstimationClientException(
                "bidcon.estimationClient.failed",
                nameof(OrganizationId),
                "Failed to retrieve Bidcon access token.");
        }

        accessTokenCache.Set(
            organizationId,
            token.AccessToken,
            TimeSpan.FromSeconds(Math.Max(1, token.ExpireInSeconds - 20)));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
        return client;
    }
}

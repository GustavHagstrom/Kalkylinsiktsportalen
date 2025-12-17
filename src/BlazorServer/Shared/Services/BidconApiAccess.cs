using BlazorServer.Shared.Constants;
using BlazorServer.Shared.DTOs.Bidcon;
using BlazorServer.Shared.Stores;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Headers;

namespace BlazorServer.Shared.Services;

public class BidconApiAccess
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMemoryCache _cache;
    private readonly IBidconApiCredentialsStore _credentialsStore;


    public BidconApiAccess(IHttpClientFactory httpClientFactory, IMemoryCache memoryCache, IBidconApiCredentialsStore credentialsStore)
    {
        _httpClientFactory = httpClientFactory;
        _cache = memoryCache;
        _credentialsStore = credentialsStore;
    }
    public async Task<EstimationDetailsDto?> GetEstimationAsync(string estimationId, string orgId)
    {
        using var client = await GetClientAsync(orgId);
        var hrefToEstimation = await client.GetStringAsync($"EstimationUri/{estimationId}");

        client.DefaultRequestHeaders.Clear();
        var estimationResponse = await client.GetFromJsonAsync<EstimationDetailsDto>(hrefToEstimation);

        return estimationResponse;
    }


    public async Task<List<EstimationDto>> GetAllEstimationsFlatAsync(string userEmail, string orgId)
    {
        using var client = await GetClientAsync(orgId);
        var response = await client.GetAsync($"Estimations/{userEmail}");
        if(response.IsSuccessStatusCode)
        {
            var estimations = await response.Content.ReadFromJsonAsync<List<EstimationDto>>();
            return estimations ?? new List<EstimationDto>();
        }
        throw new HttpRequestException($"Failed to retrieve estimations for user {userEmail} in orgId {orgId}. Status code: {response.StatusCode}");
    }
    private async Task<HttpClient> GetClientAsync(string orgId)
    {
        var client = _httpClientFactory.CreateClient(HttpClientNames.BidconApiClient);
        if (_cache.TryGetValue<string>($"bidconAccessToken_{orgId}", out var accessToken) && accessToken is not null)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return client;
        }
        else
        {
            var credentials = await _credentialsStore.GetAsync(orgId);
            if (credentials is null)
            {
                throw new InvalidOperationException($"No Bidcon API credentials found for orgId {orgId}");
            }
            var tokenResponse = await client.PostAsJsonAsync("Authentication/Authenticate", new
            {
                apiKey = credentials.ApiKey,
                secret = credentials.ApiSecret
            }).Result.Content.ReadFromJsonAsync<ApiKeyAuthenticateResultDto>();

            if(tokenResponse is null || string.IsNullOrEmpty(tokenResponse.AccessToken))
            {
                throw new InvalidOperationException($"Failed to retrieve access token for orgId {orgId}");
            }

            _cache.Set<string>($"bidconAccessToken_{orgId}", tokenResponse.AccessToken, TimeSpan.FromSeconds(tokenResponse.ExpireInSeconds - 20));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);
            return client;
        }
    }

}
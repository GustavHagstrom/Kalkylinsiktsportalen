using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;

namespace SharedModule;
/// <summary>
/// Consumed by MSAL
/// </summary>
/// <param name="tokenAcquisition"></param>
/// <param name="configuration"></param>
internal class GraphClientAuthProvider(ITokenAcquisition tokenAcquisition, IConfiguration configuration) : IAuthenticationProvider
{
    public async Task AuthenticateRequestAsync(RequestInformation request, Dictionary<string, object>? additionalAuthenticationContext = null, CancellationToken cancellationToken = default)
    {
        var scopes = configuration.GetSection("DownstreamApi")["Scopes"]?.Split(" ");
        if (scopes is not null)
        {
            var token = await tokenAcquisition.GetAccessTokenForUserAsync(scopes);
            if(token is not null)
            {
                request.Headers.Add("Authorization", $"Bearer {token}");
            }
            else
            {
                throw new InvalidOperationException("Failed to retrieve an access token for the downstream API.");
            }
        }
        else
        {
            throw new InvalidOperationException("Scopes for the downstream API are not specified in the configuration.");
        }
    }
}

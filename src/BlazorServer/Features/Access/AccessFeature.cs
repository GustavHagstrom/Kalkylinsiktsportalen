using BlazorServer.Shared.Abstractions;
using BlazorServer.Shared.Constants;
using BlazorServer.Shared.Registration;

namespace BlazorServer.Features.Access;

public class AccessFeature : IFeature
{
    public void AddAuthorizationPolicies(PolicyCollection policies)
    {
        policies.AddPolicy(FeatureConstants.Authorization.PolicyName, config =>
        {
            config.RequireAuthenticatedUser();
            config.RequireClaim(SharedApplicationClaimTypes.AccessClaim, FeatureConstants.Authorization.AccessClaimValue);
        });
    }

    public void RegisterClaims(ClaimInfoCollection collection)
    {

    }

    public void RegisterRenderObjects(RenderCollection collection)
    {

    }

    public void RegisterServices(IServiceCollection services)
    {

    }
}

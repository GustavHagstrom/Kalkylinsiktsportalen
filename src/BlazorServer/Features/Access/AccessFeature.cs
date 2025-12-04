using BlazorServer.Features.Access.Components;
using BlazorServer.Shared.Abstractions;
using BlazorServer.Shared.Constants;
using BlazorServer.Shared.Models;
using BlazorServer.Shared.Registration;

namespace BlazorServer.Features.Access;

public class AccessFeature : IFeature
{
    public void AddAuthorizationPolicies(PolicyCollection policies)
    {
        policies.AddPolicy(FeatureConstants.Authorization.PolicyName, config =>
        {
            config.RequireAssertion(context => 
                context.User.HasClaim(SharedApplicationClaimTypes.AccessClaim, FeatureConstants.Authorization.AccessClaimValue) || 
                context.User.IsInRole(AuthConstants.AdminRoleClaimValue));
        });
    }

    public void RegisterClaims(ClaimInfoCollection collection)
    {
        collection.AddAccessClaimInfo(new AccessClaimInfo()
        {
            Value = FeatureConstants.Authorization.AccessClaimValue,
            Name = "Rättigheter",
            Description = "Skapa, ändra och ta bort användarrättigheter och roller."
        });
    }

    public void RegisterRenderObjects(RenderCollection collection)
    {
        collection.AddSettingNavigationComponent<SettingsLink>();
    }

    public void RegisterServices(IServiceCollection services)
    {

    }
}

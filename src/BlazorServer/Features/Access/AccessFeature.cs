using BlazorServer.Features.Access.Components;
using BlazorServer.Shared.Abstractions;
using BlazorServer.Shared.Constants;
using BlazorServer.Shared.Models;
using BlazorServer.Shared.Composition;
using BlazorServer.Shared.FeatureRegistration;

namespace BlazorServer.Features.Access;

public class AccessFeature : IFeature
{
    public void AddAuthorizationPolicies(PolicyCollection policies)
    {
        policies.AddPolicy(FeatureConstants.Authorization.PolicyName, config =>
        {
            config.RequireAssertion(context => 
                context.User.HasClaim(ApplicationClaimTypes.AccessClaim, FeatureConstants.Authorization.AccessClaimValue) || 
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
        collection.AddSettingsNavigation(
            text: "Rättigheter",
            href: FeatureConstants.Routes.SettingsPage,
            order: 10,
            icon: MudBlazor.Icons.Material.Sharp.AdminPanelSettings,
            policy: FeatureConstants.Authorization.PolicyName);
    }

    public void RegisterServices(IServiceCollection services)
    {

    }
}

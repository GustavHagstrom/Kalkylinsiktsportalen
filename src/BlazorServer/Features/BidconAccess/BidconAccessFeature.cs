using BlazorServer.Shared.Abstractions;
using BlazorServer.Shared.Constants;
using BlazorServer.Shared.Models;
using BlazorServer.Shared.Composition;
using BlazorServer.Shared.FeatureRegistration;

namespace BlazorServer.Features.BidconAccess;

public class BidconAccessFeature : IFeature
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
        collection.AddAccessClaimInfo(new AccessClaimInfo
        {
            Name = "BidconApiAccess",
            Value = FeatureConstants.Authorization.AccessClaimValue,
            Description = "Ändra Api-nyckel och hemlighet."
        });
    }

    public void RegisterRenderObjects(RenderCollection collection)
    {
        collection.AddSettingsNavigation(
            text: "Api",
            href: FeatureConstants.Routes.SettingsPage,
            order: 20,
            icon: MudBlazor.Icons.Material.Sharp.Api,
            policy: FeatureConstants.Authorization.PolicyName);
    }

    public void RegisterServices(IServiceCollection services)
    {

    }
}

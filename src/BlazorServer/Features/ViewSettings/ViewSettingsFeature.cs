using BlazorServer.Features.ViewSettings.Components;
using BlazorServer.Shared.Abstractions;
using BlazorServer.Shared.Constants;
using BlazorServer.Shared.Models;
using BlazorServer.Shared.Registration;

namespace BlazorServer.Features.ViewSettings;

public class ViewSettingsFeature : IFeature
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
            Name = "ViewSettingsAccess",
            Value = FeatureConstants.Authorization.AccessClaimValue,
            Description = "Tillåts redigera mallarna och sätta rättigheter på dem."
        });
    }

    public void RegisterRenderObjects(RenderCollection collection)
    {
        collection.AddSettingsNavigation(
            text: "Vyer",
            href: FeatureConstants.Routes.SettingsPage,
            order: 30,
            icon: MudBlazor.Icons.Material.Sharp.Article,
            policy: FeatureConstants.Authorization.PolicyName);
    }

    public void RegisterServices(IServiceCollection services)
    {

    }
}

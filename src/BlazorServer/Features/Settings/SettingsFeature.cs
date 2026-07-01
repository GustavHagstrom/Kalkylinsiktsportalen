using BlazorServer.Shared.Abstractions;
using BlazorServer.Shared.Composition;
using BlazorServer.Shared.FeatureRegistration;
using BlazorServer.Shared.ModuleRegistration;

namespace BlazorServer.Features.Settings;

public class SettingsFeature : IFeature
{

    public void AddAuthorizationPolicies(PolicyCollection policies)
    {
        
    }

    public void RegisterClaims(ClaimInfoCollection collection)
    {
    }

    public void RegisterRenderObjects(RenderCollection collection)
    {
        collection.AddTopNavigation(
            text: "Inställningar",
            href: BlazorServer.Shared.Constants.AppRoutes.Settings,
            order: 10);
    }

    public void RegisterServices(IServiceCollection services)
    {

    }
}

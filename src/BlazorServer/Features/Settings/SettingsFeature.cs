using BlazorServer.Shared.Abstractions;
using BlazorServer.Shared.Registration;

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
        collection.AddAppBarNavMenuComponent<AppNavMenue>(10);
    }

    public void RegisterServices(IServiceCollection services)
    {

    }
}

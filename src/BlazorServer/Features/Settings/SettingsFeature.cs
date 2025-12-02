using BlazorServer.Shared.Abstractions;
using BlazorServer.Shared.Constants;
using BlazorServer.Shared.Registration;

namespace BlazorServer.Features.Settings;

public class SettingsFeature : IFeature
{
    public void RegisterRenderObjects(RenderCollection collection)
    {
        collection.AddAppBarNavMenuComponent<AppNavMenue>(10);
    }

    public void RegisterServices(IServiceCollection services)
    {

    }
}

using Microsoft.AspNetCore.Components;

namespace BlazorServer.Shared.Registration;

public sealed class AppShellRegistration
{
    private readonly RenderCollection renderCollection;

    public AppShellRegistration(RenderCollection renderCollection)
    {
        this.renderCollection = renderCollection;
    }

    public void AddTopNavigation<TComponent>(int order)
        where TComponent : ComponentBase
    {
        renderCollection.AddAppBarNavMenuComponent<TComponent>(order);
    }

    public void AddSettingsNavigation<TComponent>()
        where TComponent : ComponentBase
    {
        renderCollection.AddSettingNavigationComponent<TComponent>();
    }

    public void AddSettingsSection<TComponent>()
        where TComponent : ComponentBase
    {
        renderCollection.AddSharedSettingComponent<TComponent>();
    }

    public void SetLoginDisplay<TComponent>()
        where TComponent : ComponentBase
    {
        renderCollection.SetLoginDisplayComponent<TComponent>();
    }
}

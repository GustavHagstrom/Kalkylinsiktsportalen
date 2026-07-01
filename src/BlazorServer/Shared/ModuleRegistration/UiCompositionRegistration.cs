using BlazorServer.Shared.Composition;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace BlazorServer.Shared.ModuleRegistration;

public sealed class UiCompositionRegistration
{
    private readonly UiCompositionRegistry registry;

    public UiCompositionRegistration(UiCompositionRegistry registry)
        : this(registry, Guid.NewGuid().ToString("N"))
    {
    }

    public UiCompositionRegistration(UiCompositionRegistry registry, string moduleKey)
    {
        this.registry = registry;
        Settings = new UiSettingsRegistration(registry, moduleKey);
    }

    public UiSettingsRegistration Settings { get; }

    public void AddTopNavigation(
        string text,
        string href,
        int order,
        string? icon = null,
        string? policy = null,
        NavLinkMatch match = NavLinkMatch.Prefix)
    {
        registry.AddTopNavigation(new UiNavigationItem(text, href, order, icon, policy, match));
    }

    public void SetLoginDisplay<TComponent>()
        where TComponent : ComponentBase
    {
        registry.SetLoginDisplay(typeof(TComponent));
    }
}

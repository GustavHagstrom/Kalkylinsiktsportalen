using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace BlazorServer.Shared.Registration;

public sealed class UiCompositionRegistration
{
    private readonly UiCompositionRegistry registry;

    public UiCompositionRegistration(UiCompositionRegistry registry)
    {
        this.registry = registry;
    }

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

    public void AddSettingsNavigation(
        string text,
        string href,
        int order,
        string? icon = null,
        string? policy = null,
        NavLinkMatch match = NavLinkMatch.Prefix)
    {
        registry.AddSettingsNavigation(new UiNavigationItem(text, href, order, icon, policy, match));
    }

    public void AddSettingsSection<TComponent>(int order)
        where TComponent : ComponentBase
    {
        registry.AddSettingsSection(typeof(TComponent), order);
    }

    public void SetLoginDisplay<TComponent>()
        where TComponent : ComponentBase
    {
        registry.SetLoginDisplay(typeof(TComponent));
    }
}

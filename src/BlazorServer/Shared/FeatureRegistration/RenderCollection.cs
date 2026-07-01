using BlazorServer.Shared.Composition;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace BlazorServer.Shared.FeatureRegistration;

public class RenderCollection
{
    private readonly UiCompositionRegistry uiComposition;
    private readonly List<(Type Type, int Order)> legacyAppBarNavigationComponents = [];
    private readonly List<Type> legacySettingNavigationComponents = [];

    public RenderCollection()
        : this(new UiCompositionRegistry())
    {
    }

    public RenderCollection(UiCompositionRegistry uiComposition)
    {
        this.uiComposition = uiComposition;
    }

    public void AddTopNavigation(
        string text,
        string href,
        int order,
        string? icon = null,
        string? policy = null,
        NavLinkMatch match = NavLinkMatch.Prefix)
    {
        uiComposition.AddTopNavigation(new UiNavigationItem(text, href, order, icon, policy, match));
    }

    public void AddSettingsNavigation(
        string text,
        string href,
        int order,
        string? icon = null,
        string? policy = null,
        NavLinkMatch match = NavLinkMatch.Prefix)
    {
        uiComposition.AddSettingsNavigation(new UiNavigationItem(text, href, order, icon, policy, match));
    }

    public void AddSharedSettingComponent<T>(int order = 0)
        where T : ComponentBase
    {
        uiComposition.AddSettingsSection(typeof(T), order);
    }

    public void SetLoginDisplayComponent<T>()
        where T : ComponentBase
    {
        uiComposition.SetLoginDisplay(typeof(T));
    }

    public Type? GetLoginDisplayComponent() => uiComposition.LoginDisplayComponent;

    [Obsolete("Use AddTopNavigation with navigation data instead.")]
    public void AddAppBarNavMenuComponent<T>(int order)
        where T : ComponentBase
    {
        legacyAppBarNavigationComponents.Add((typeof(T), order));
    }

    [Obsolete("Use UiCompositionRegistry.TopNavigation instead.")]
    public IEnumerable<(Type Type, int Order)> GetAppBarNavMenueComponents() =>
        legacyAppBarNavigationComponents;

    [Obsolete("Use UiCompositionRegistry.SettingsSections instead.")]
    public IEnumerable<Type> GetSharedSettingComponents() =>
        uiComposition.SettingsSections.Select(component => component.ComponentType);

    [Obsolete("Use AddSettingsNavigation with navigation data instead.")]
    public void AddSettingNavigationComponent<T>()
        where T : ComponentBase
    {
        legacySettingNavigationComponents.Add(typeof(T));
    }

    [Obsolete("Use UiCompositionRegistry.SettingsNavigation instead.")]
    public IEnumerable<Type> GetSettingNavigationComponents() => legacySettingNavigationComponents;
}

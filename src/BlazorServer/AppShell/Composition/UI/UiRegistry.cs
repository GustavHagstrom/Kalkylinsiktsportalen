using BlazorServer.AppShell.Composition.Navigation;
using BlazorServer.AppShell.Composition.Settings;
using Microsoft.AspNetCore.Components;

namespace BlazorServer.AppShell.Composition.UI;

public sealed class UiRegistry
{
    private readonly NavigationRegistry navigation = new();
    private readonly SettingsAreaRegistry settingsAreaRegistry = new();

    public NavigationRegistry Navigation => navigation;

    public SettingsAreaRegistry SettingsAreaRegistry => settingsAreaRegistry;

    public IReadOnlyList<NavigationItem> TopNavigation => navigation.TopNavigation;

    public IReadOnlyList<NavigationItem> SettingsNavigation => navigation.SettingsNavigation;

    public IReadOnlyList<SettingsAreaContribution> SettingsAreas => settingsAreaRegistry.SettingsAreas;

    public Type? LoginDisplayComponent { get; private set; }

    public void AddTopNavigation(NavigationItem item)
    {
        navigation.AddTopNavigation(item);
    }

    public void AddSettingsNavigation(NavigationItem item)
    {
        navigation.AddSettingsNavigation(item);
    }

    public void AddSettingsArea(SettingsAreaContribution area, string moduleKey)
    {
        settingsAreaRegistry.Add(area, moduleKey);
    }

    public void SetLoginDisplay<T>()
        where T : ComponentBase
    {
        var type = typeof(T);
        if (LoginDisplayComponent is not null && LoginDisplayComponent != type)
        {
            throw new InvalidOperationException(
                $"Login display component is already registered as {LoginDisplayComponent.FullName}.");
        }

        LoginDisplayComponent = type;
    }
}

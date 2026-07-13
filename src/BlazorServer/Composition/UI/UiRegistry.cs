using BlazorServer.Composition.Navigation;
using BlazorServer.Composition.Settings;
using Microsoft.AspNetCore.Components;

namespace BlazorServer.Composition.UI;

public sealed class UiRegistry
{
    private readonly NavigationRegistry navigation = new();
    private readonly SettingsAreaRegistry settingsAreaRegistry = new();

    public NavigationRegistry Navigation => navigation;

    public SettingsAreaRegistry SettingsAreaRegistry => settingsAreaRegistry;

    public IReadOnlyList<NavigationItem> TopNavigation => navigation.TopNavigation;

    public IReadOnlyList<NavigationItem> SettingsNavigation => navigation.SettingsNavigation;

    public IReadOnlyList<SettingsAreaContribution> SettingsAreas => settingsAreaRegistry.SettingsAreas;

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

   
}

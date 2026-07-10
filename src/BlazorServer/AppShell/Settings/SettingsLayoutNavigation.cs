using BlazorServer.AppShell.Composition.Navigation;
using BlazorServer.AppShell.Composition.UI;
using BlazorServer.AppShell.Composition;

namespace BlazorServer.AppShell.Settings;

public static class SettingsLayoutNavigation
{
    public static IReadOnlyList<NavigationItem> GetNavigationItems(UiRegistry ui)
    {
        return [];
    }
}

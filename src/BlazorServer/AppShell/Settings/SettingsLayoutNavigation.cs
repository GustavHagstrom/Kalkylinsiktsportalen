using BlazorServer.Composition.Navigation;
using BlazorServer.Composition.UI;

namespace BlazorServer.AppShell.Settings;

public static class SettingsLayoutNavigation
{
    public static IReadOnlyList<NavigationItem> GetNavigationItems(UiRegistry ui)
    {
        return [];
    }
}

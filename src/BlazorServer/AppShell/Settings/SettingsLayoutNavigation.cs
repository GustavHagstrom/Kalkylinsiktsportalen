using BlazorServer.Shared.Composition;

namespace BlazorServer.AppShell.Settings;

public static class SettingsLayoutNavigation
{
    public static IReadOnlyList<UiNavigationItem> GetNavigationItems(UiCompositionRegistry uiComposition)
    {
        return [];
    }
}

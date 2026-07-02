using BlazorServer.Shared.Composition;

namespace BlazorServer.Features.Settings;

public static class SettingsLayoutNavigation
{
    public static IReadOnlyList<UiNavigationItem> GetNavigationItems(UiCompositionRegistry uiComposition)
    {
        return [];
    }
}

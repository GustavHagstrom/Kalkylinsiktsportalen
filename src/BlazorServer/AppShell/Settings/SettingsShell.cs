using BlazorServer.Shared.Composition;
using BlazorServer.Shared.Constants;

namespace BlazorServer.AppShell.Settings;

public static class SettingsShell
{
    public static IServiceCollection AddSettingsShell(
        this IServiceCollection services,
        AppRegistrationContext registrationContext)
    {
        registrationContext.UiComposition.AddTopNavigation(
            new UiNavigationItem(
                "Inställningar",
                AppRoutes.Settings,
                10));

        return services;
    }
}

using BlazorServer.AppShell.Composition.Navigation;
using BlazorServer.AppShell.Composition;
using BlazorServer.AppShell.Routes;

namespace BlazorServer.AppShell.Settings;

public static class SettingsShell
{
    public static IServiceCollection AddSettingsShell(
        this IServiceCollection services,
        AppRegistrationContext registrationContext)
    {
        registrationContext.Ui.AddTopNavigation(
            new NavigationItem(
                "Inställningar",
                AppShellRoutes.Settings,
                10));

        return services;
    }
}

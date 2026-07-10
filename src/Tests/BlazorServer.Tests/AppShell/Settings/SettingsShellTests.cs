using BlazorServer.AppShell.Composition.Navigation;
using BlazorServer.AppShell.Composition.UI;
using BlazorServer.AppShell.Settings;
using BlazorServer.AppShell.Composition;
using BlazorServer.AppShell.Routes;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorServer.Tests.AppShell.Settings;

[TestFixture]
public class SettingsShellTests
{
    [Test]
    public void SettingsShellRegistersAppbarNavigationToSettingsHub()
    {
        var services = new ServiceCollection();
        var registrationContext = services.AddAppRegistrationContext();

        services.AddSettingsShell(registrationContext);

        Assert.That(registrationContext.Ui.TopNavigation, Has.One.EqualTo(
            new NavigationItem(
                "Inställningar",
                AppShellRoutes.Settings,
                10)));
    }

    [Test]
    public void SettingsShellNavigationDoesNotIncludeLegacySettingsNavigation()
    {
        var registry = new UiRegistry();
        registry.AddSettingsNavigation(new NavigationItem("Legacy settings", "/settings/legacy", 10));

        var navigationItems = SettingsLayoutNavigation.GetNavigationItems(registry);

        Assert.That(navigationItems, Is.Empty);
    }
}

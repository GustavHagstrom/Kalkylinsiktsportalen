using BlazorServer.AppShell.Settings;
using BlazorServer.Shared.Composition;
using BlazorServer.Shared.Constants;
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

        Assert.That(registrationContext.UiComposition.TopNavigation, Has.One.EqualTo(
            new UiNavigationItem(
                "Inställningar",
                AppRoutes.Settings,
                10)));
    }

    [Test]
    public void SettingsShellNavigationDoesNotIncludeLegacySettingsNavigation()
    {
        var registry = new UiCompositionRegistry();
        registry.AddSettingsNavigation(new UiNavigationItem("Legacy settings", "/settings/legacy", 10));

        var navigationItems = SettingsLayoutNavigation.GetNavigationItems(registry);

        Assert.That(navigationItems, Is.Empty);
    }
}

using BlazorServer.Features.Settings;
using BlazorServer.Shared.Composition;

namespace BlazorServer.Tests.Features.Settings;

[TestFixture]
public class SettingsLayoutNavigationTests
{
    [Test]
    public void SettingsShellNavigationDoesNotIncludeLegacySettingsNavigation()
    {
        var registry = new UiCompositionRegistry();
        registry.AddSettingsNavigation(new UiNavigationItem("Legacy settings", "/settings/legacy", 10));

        var navigationItems = SettingsLayoutNavigation.GetNavigationItems(registry);

        Assert.That(navigationItems, Is.Empty);
    }
}

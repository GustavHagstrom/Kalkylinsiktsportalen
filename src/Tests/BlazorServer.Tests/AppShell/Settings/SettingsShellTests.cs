using BlazorServer.Composition.Navigation;
using BlazorServer.Composition.UI;
using BlazorServer.AppShell.Routes;
using BlazorServer.AppShell.Settings;
using BlazorServer.AppShell.Settings.Infrastructure.MongoDb.Entities;
using BlazorServer.AppShell.Settings.Infrastructure.MongoDb.Stores;
using BlazorServer.Composition;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

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
    public void SettingsShellRegistersAppOwnedPersistence()
    {
        var services = new ServiceCollection();
        var registrationContext = services.AddAppRegistrationContext();

        services.AddSettingsShell(registrationContext);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(services, Has.One.Matches<ServiceDescriptor>(service =>
                service.ServiceType == typeof(IMongoCollection<MongoUserSettings>) &&
                service.Lifetime == ServiceLifetime.Singleton));
            Assert.That(services, Has.One.Matches<ServiceDescriptor>(service =>
                service.ServiceType == typeof(IUserSettingsStore) &&
                service.ImplementationType == typeof(MongoUserSettingsStore) &&
                service.Lifetime == ServiceLifetime.Singleton));
            Assert.That(services, Has.One.Matches<ServiceDescriptor>(service =>
                service.ServiceType == typeof(IDarkmodeStore) &&
                service.ImplementationType == typeof(MongoUserSettingsStore) &&
                service.Lifetime == ServiceLifetime.Singleton));
        }
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

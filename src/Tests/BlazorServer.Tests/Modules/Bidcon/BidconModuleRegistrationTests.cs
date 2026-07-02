using BlazorServer.Modules.Bidcon;
using BlazorServer.Modules.Bidcon.Features.ImportEstimationSnapshot;
using BlazorServer.Modules.Bidcon.Features.UpsertBidconCredentials;
using BlazorServer.Modules.Bidcon.Infrastructure.Persistence;
using BlazorServer.Shared.Composition;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorServer.Tests.Modules.Bidcon;

[TestFixture]
public class BidconModuleRegistrationTests
{
    [Test]
    public void ServicesCanRegisterBidconModule()
    {
        var services = new ServiceCollection();
        var registrationContext = services.AddAppRegistrationContext();

        services.AddBidconModule(registrationContext);

        Assert.Multiple(() =>
        {
            Assert.That(services.Any(service => service.ServiceType == typeof(UpsertBidconCredentialsCommandHandler)), Is.True);
            Assert.That(services.Any(service => service.ServiceType == typeof(ImportEstimationSnapshotCommandHandler)), Is.True);
            Assert.That(services.Any(service => service.ServiceType == typeof(IBidconAccessTokenCache)), Is.True);
            Assert.That(services.Any(service => service.ServiceType == typeof(IBidconApiCredentialsStore)), Is.True);
            Assert.That(services.Any(service => service.ServiceType == typeof(IEstimationSnapshotStore)), Is.True);
            Assert.That(registrationContext.UiComposition.SettingsNavigation, Is.Empty);
        });
    }
}

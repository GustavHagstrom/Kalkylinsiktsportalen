using BlazorServer.Composition;
using BlazorServer.Kernel.CQRS;
using BlazorServer.Modules.Reports;
using BlazorServer.Modules.Reports.Features.CreateView;
using BlazorServer.Modules.Reports.Features.GetView;
using BlazorServer.Modules.Reports.Features.ListViews;
using BlazorServer.Modules.Reports.Views;
using BlazorServer.Modules.Reports.Views.Infrastructure.MongoDb;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace BlazorServer.Tests.Modules.Reports;

[TestFixture]
public class ReportsModuleRegistrationTests
{
    [Test]
    public void ServicesCanRegisterReportsViewPersistence()
    {
        var services = new ServiceCollection();
        var registrationContext = services.AddAppRegistrationContext();

        services.AddReportsModule(registrationContext);

        Assert.Multiple(() =>
        {
            Assert.That(services, Has.One.Matches<ServiceDescriptor>(service =>
                service.ServiceType == typeof(IMongoCollection<MongoView>) &&
                service.Lifetime == ServiceLifetime.Singleton));
            Assert.That(services, Has.One.Matches<ServiceDescriptor>(service =>
                service.ServiceType == typeof(IViewStore) &&
                service.ImplementationType == typeof(MongoViewStore) &&
                service.Lifetime == ServiceLifetime.Singleton));
            Assert.That(services, Has.One.Matches<ServiceDescriptor>(service =>
                service.ServiceType == typeof(ICommandHandler<CreateViewCommand, CreateViewResult>) &&
                service.ImplementationType == typeof(CreateViewCommandHandler) &&
                service.Lifetime == ServiceLifetime.Scoped));
            Assert.That(services, Has.One.Matches<ServiceDescriptor>(service =>
                service.ServiceType == typeof(IQueryHandler<GetViewQuery, GetViewResult>) &&
                service.ImplementationType == typeof(GetViewQueryHandler) &&
                service.Lifetime == ServiceLifetime.Scoped));
            Assert.That(services, Has.One.Matches<ServiceDescriptor>(service =>
                service.ServiceType == typeof(IQueryHandler<ListViewsQuery, ListViewsResult>) &&
                service.ImplementationType == typeof(ListViewsQueryHandler) &&
                service.Lifetime == ServiceLifetime.Scoped));
        });
    }
}

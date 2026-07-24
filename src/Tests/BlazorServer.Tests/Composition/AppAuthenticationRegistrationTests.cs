using BlazorServer;
using BlazorServer.Composition;
using BlazorServer.AppShell.Authentication.Entra;
using BlazorServer.AppShell.Authentication.Mock;
using BlazorServer.Modules.Access.Infrastructure.Identity;
using BlazorServer.Modules.Access.Public.Authorization;
using BlazorServer.Modules.Access.Public.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorServer.Tests.Composition;

[TestFixture]
public class AppAuthenticationRegistrationTests
{
    [Test]
    public void AppAuthenticationRegistersMockAuthenticationAndMockAccessDirectoryForMockMode()
    {
        var services = new ServiceCollection();
        var registrationContext = new AppRegistrationContext();
        var configuration = Configuration(new Dictionary<string, string?>
        {
            ["Auth:Mode"] = "Mock",
            ["Auth:Mock:OrganizationId"] = "org-1"
        });

        services.AddAppAuthentication(configuration, registrationContext);

        using var provider = services.BuildServiceProvider();

        Assert.That(provider.GetRequiredService<IAccessDirectory>(), Is.TypeOf<MockAccessDirectory>());
        Assert.That(provider.GetRequiredService<IApplicationAdminPrincipalEvaluator>(), Is.TypeOf<MockApplicationAdminPrincipalEvaluator>());
    }

    [Test]
    public void AppAuthenticationRegistersEntraAuthenticationAndGraphAccessDirectoryForEntraMode()
    {
        var services = new ServiceCollection();
        var registrationContext = new AppRegistrationContext();
        var configuration = Configuration(new Dictionary<string, string?>
        {
            ["Auth:Mode"] = "EntraId",
            ["AzureAd:Instance"] = "https://login.microsoftonline.com/",
            ["AzureAd:TenantId"] = "tenant",
            ["AzureAd:ClientId"] = "client",
            ["DownstreamApi:Scopes"] = "User.Read"
        });

        services.AddAppAuthentication(configuration, registrationContext);

        Assert.That(services, Has.One.Matches<ServiceDescriptor>(descriptor =>
            descriptor.ServiceType == typeof(IAccessDirectory) &&
            descriptor.ImplementationType == typeof(GraphAccessDirectory) &&
            descriptor.Lifetime == ServiceLifetime.Transient));
        Assert.That(services, Has.One.Matches<ServiceDescriptor>(descriptor =>
            descriptor.ServiceType == typeof(IApplicationAdminPrincipalEvaluator) &&
            descriptor.ImplementationType == typeof(EntraApplicationAdminPrincipalEvaluator) &&
            descriptor.Lifetime == ServiceLifetime.Singleton));
    }

    private static IConfiguration Configuration(Dictionary<string, string?> values)
        => new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();
}

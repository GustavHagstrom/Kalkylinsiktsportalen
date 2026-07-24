using BlazorServer.Composition;
using BlazorServer.Modules.Access;
using BlazorServer.Modules.Access.Public.Authorization;
using BlazorServer.Modules.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorServer.Tests.Composition;

[TestFixture]
public class KipModuleRegistrationTests
{
    [Test]
    public void KipModulesRegisterAccessBeforeModulesThatUseAccessAuthorizationPolicies()
    {
        var services = new ServiceCollection();
        var registrationContext = services.AddAppRegistrationContext();

        services.AddKipModules(Configuration(), registrationContext);

        var policyNames = registrationContext.Policies.PolicyList
            .Select(policy => policy.Name)
            .ToArray();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(policyNames, Is.EqualTo([
                Constants.Authorization.PolicyName,
                "ApiPolicy",
                ReportsModule.Authorization.PolicyName
            ]));
            Assert.That(services, Has.One.Matches<ServiceDescriptor>(descriptor =>
                descriptor.ServiceType == typeof(IAuthorizationHandler) &&
                descriptor.ImplementationType?.Name == "AccessClaimOrApplicationAdminHandler" &&
                descriptor.Lifetime == ServiceLifetime.Transient));
        }
    }

    private static IConfiguration Configuration()
        => new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Bidcon:BaseUrl"] = "https://bidcon.example"
            })
            .Build();
}

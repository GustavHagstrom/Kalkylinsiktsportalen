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
        services.AddSingleton<IApplicationAdminPrincipalEvaluator>(new AlwaysFalseApplicationAdminPrincipalEvaluator());

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

    [Test]
    public void KipModulesFailFastWhenAuthenticationHasNotRegisteredApplicationAdminEvaluator()
    {
        var services = new ServiceCollection();
        var registrationContext = services.AddAppRegistrationContext();

        var exception = Assert.Throws<InvalidOperationException>(() =>
            services.AddKipModules(Configuration(), registrationContext));

        Assert.That(exception!.Message, Does.Contain(nameof(IApplicationAdminPrincipalEvaluator)));
    }

    private static IConfiguration Configuration()
        => new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Bidcon:BaseUrl"] = "https://bidcon.example"
            })
            .Build();

    private sealed class AlwaysFalseApplicationAdminPrincipalEvaluator : IApplicationAdminPrincipalEvaluator
    {
        public bool IsApplicationAdmin(System.Security.Claims.ClaimsPrincipal principal)
            => false;
    }
}

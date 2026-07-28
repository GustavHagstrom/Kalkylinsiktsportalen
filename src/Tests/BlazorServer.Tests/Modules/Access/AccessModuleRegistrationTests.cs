using BlazorServer.Composition.Settings;
using System.Security.Claims;
using BlazorServer.Modules.Access;
using BlazorServer.Modules.Access.Authentication;
using BlazorServer.Modules.Access.Components;
using BlazorServer.Modules.Access.Features.FunctionalAccessGrants;
using BlazorServer.Modules.Access.Features.ViewAccessGrants;
using BlazorServer.Modules.Access.Infrastructure.MongoDb.Entities;
using BlazorServer.Modules.Access.Infrastructure.MongoDb.Stores;
using BlazorServer.AppShell.Settings;
using BlazorServer.Modules.Access.Public.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using AccessSettingsPage = BlazorServer.Modules.Access.Components.SettingsPage;
using BlazorServer.Modules.Access.Public.Contracts;
using BlazorServer.Composition;

namespace BlazorServer.Tests.Modules.Access;

[TestFixture]
public class AccessModuleRegistrationTests
{
    [Test]
    public void ServicesCanRegisterAccessModule()
    {
        var services = new ServiceCollection();
        var registrationContext = services.AddAppRegistrationContext();
        services.AddAccessModule(registrationContext);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(registrationContext.AccessClaims.Claims, Has.One.EqualTo(
                new AccessClaimDefinition(
                    Constants.Authorization.AccessClaimValue,
                    "Rättigheter",
                    "Skapa, ändra och ta bort användarrättigheter och roller.")));
            Assert.That(registrationContext.Policies.PolicyList.Select(policy => policy.Name), Has.One.EqualTo(
                Constants.Authorization.PolicyName));
            Assert.That(registrationContext.Ui.SettingsNavigation, Is.Empty);
            Assert.That(registrationContext.Ui.SettingsAreas, Has.One.EqualTo(
                new SettingsAreaContribution(
                    "access",
                    "Rättigheter",
                    "Hantera användarrättigheter och externa grupper.",
                    "/settings/access",
                    10,
                    MudBlazor.Icons.Material.Sharp.AdminPanelSettings,
                    Constants.Authorization.PolicyName)));
            Assert.That(services.Any(service => service.ServiceType == typeof(FunctionalAccessGrantHandler)), Is.True);
            Assert.That(services.Any(service => service.ServiceType == typeof(GetUserFunctionalAccessQueryHandler)), Is.True);
            Assert.That(services.Any(service => service.ServiceType == typeof(GrantUserFunctionalAccessCommandHandler)), Is.True);
            Assert.That(services.Any(service => service.ServiceType == typeof(RevokeUserFunctionalAccessCommandHandler)), Is.True);
            Assert.That(services, Has.One.Matches<ServiceDescriptor>(service =>
                service.ServiceType == typeof(IMongoCollection<MongoFunctionalAccessGrant>) &&
                service.Lifetime == ServiceLifetime.Singleton));
            Assert.That(services, Has.One.Matches<ServiceDescriptor>(service =>
                service.ServiceType == typeof(IFunctionalAccessGrantStore) &&
                service.ImplementationType == typeof(MongoAccessStore) &&
                service.Lifetime == ServiceLifetime.Singleton));
            Assert.That(services.Any(service => service.ServiceType == typeof(IViewAccessGrantStore)), Is.True);
            Assert.That(services.Any(service => service.ServiceType == typeof(ViewAccessGrantHandler)), Is.True);
            Assert.That(services.Any(service => service.ServiceType == typeof(ViewAccessDecisionHandler)), Is.True);
            Assert.That(services, Has.One.Matches<ServiceDescriptor>(service =>
                service.ServiceType == typeof(IAccessSubjectFactory) &&
                service.ImplementationType == typeof(AccessSubjectFactory) &&
                service.Lifetime == ServiceLifetime.Transient));
            Assert.That(services.Any(service => service.ServiceType == typeof(IApplicationAdminPrincipalEvaluator)), Is.False);
        }
    }

    [Test]
    public async Task AccessSettingsAreaVisibilityUsesAccessPolicy()
    {
        var services = new ServiceCollection();
        var registrationContext = services.AddAppRegistrationContext();

        services.AddAccessModule(registrationContext);
        services.AddLogging();
        services.AddSingleton<IApplicationAdminPrincipalEvaluator>(
            new ClaimTypeApplicationAdminPrincipalEvaluator("external-admin"));
        services.ApplyAppRegistrationContext(registrationContext);

        using var provider = services.BuildServiceProvider();
        var authorizationService = provider.GetRequiredService<IAuthorizationService>();

        var visibleForClaim = await SettingsHub.GetVisibleAreasAsync(
            registrationContext.Ui,
            PrincipalWithClaim(),
            authorizationService);
        var visibleForAdmin = await SettingsHub.GetVisibleAreasAsync(
            registrationContext.Ui,
            PrincipalInAdminRole(),
            authorizationService);
        var hiddenForUnprivilegedUser = await SettingsHub.GetVisibleAreasAsync(
            registrationContext.Ui,
            new ClaimsPrincipal(new ClaimsIdentity("test")),
            authorizationService);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(visibleForClaim.Select(area => area.Id), Has.One.EqualTo("access"));
            Assert.That(visibleForAdmin.Select(area => area.Id), Has.One.EqualTo("access"));
            Assert.That(hiddenForUnprivilegedUser, Is.Empty);
        };
    }

    [Test]
    public void AccessSettingsPagesUseModuleRoutesWithAuthorizationPolicy()
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(RouteTemplatesFor<AccessSettingsPage>(), Is.EquivalentTo(["/settings/access"]));
            Assert.That(RouteTemplatesFor<EditUserAccessPage>(), Is.EquivalentTo(["/settings/access/users/{UserId}"]));
            Assert.That(RouteTemplatesFor<EditRoleAccessPage>(), Is.EquivalentTo(["/settings/access/roles/{RoleId}"]));
            Assert.That(AuthorizationPoliciesFor<AccessSettingsPage>(), Has.One.EqualTo(Constants.Authorization.PolicyName));
            Assert.That(AuthorizationPoliciesFor<EditUserAccessPage>(), Has.One.EqualTo(Constants.Authorization.PolicyName));
            Assert.That(AuthorizationPoliciesFor<EditRoleAccessPage>(), Has.One.EqualTo(Constants.Authorization.PolicyName));
        };
    }

    private static ClaimsPrincipal PrincipalWithClaim()
    {
        return new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(AccessAuthorizationConstants.AccessClaimType, Constants.Authorization.AccessClaimValue)
        ], "test"));
    }

    private static ClaimsPrincipal PrincipalInAdminRole()
    {
        return new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(
                "external-admin",
                "true")
        ], "test"));
    }

    private sealed class ClaimTypeApplicationAdminPrincipalEvaluator(string claimType) : IApplicationAdminPrincipalEvaluator
    {
        public bool IsApplicationAdmin(ClaimsPrincipal principal)
            => principal.HasClaim(claimType, "true");
    }

    private static string[] RouteTemplatesFor<TComponent>()
    {
        return [.. typeof(TComponent)
            .GetCustomAttributes(typeof(RouteAttribute), inherit: false)
            .Cast<RouteAttribute>()
            .Select(attribute => attribute.Template)];
    }

    private static string?[] AuthorizationPoliciesFor<TComponent>()
    {
        return [.. typeof(TComponent)
            .GetCustomAttributes(typeof(AuthorizeAttribute), inherit: false)
            .Cast<AuthorizeAttribute>()
            .Select(attribute => attribute.Policy)];
    }
}

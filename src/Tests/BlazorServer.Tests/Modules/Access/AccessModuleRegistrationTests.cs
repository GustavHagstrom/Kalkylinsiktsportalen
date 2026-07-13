using BlazorServer.AppShell.Composition.Settings;
using System.Security.Claims;
using BlazorServer.Modules.Access;
using BlazorServer.Modules.Access.Components;
using BlazorServer.Modules.Access.Features.FunctionalAccessGrants;
using BlazorServer.Modules.Access.Features.ViewAccessGrants;
using BlazorServer.Modules.Access.Infrastructure.MongoDb.Entities;
using BlazorServer.Modules.Access.Infrastructure.MongoDb.Stores;
using BlazorServer.AppShell.Settings;
using BlazorServer.AppShell.Composition;
using BlazorServer.Modules.Access.Public.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using AccessSettingsPage = BlazorServer.Modules.Access.Components.SettingsPage;
using BlazorServer.Modules.Access.Public.Contracts;

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

        Assert.Multiple(() =>
        {
            Assert.That(registrationContext.AccessClaims.Claims, Has.One.EqualTo(
                new AccessClaimInfo(
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
        });
    }

    [Test]
    public async Task AccessSettingsAreaVisibilityUsesAccessPolicy()
    {
        var services = new ServiceCollection();
        var registrationContext = services.AddAppRegistrationContext();

        services.AddAccessModule(registrationContext);
        services.AddLogging();
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

        Assert.Multiple(() =>
        {
            Assert.That(visibleForClaim.Select(area => area.Id), Has.One.EqualTo("access"));
            Assert.That(visibleForAdmin.Select(area => area.Id), Has.One.EqualTo("access"));
            Assert.That(hiddenForUnprivilegedUser, Is.Empty);
        });
    }

    [Test]
    public void AccessSettingsPagesUseModuleRoutesWithAuthorizationPolicy()
    {
        Assert.Multiple(() =>
        {
            Assert.That(RouteTemplatesFor<AccessSettingsPage>(), Is.EquivalentTo(["/settings/access"]));
            Assert.That(RouteTemplatesFor<EditUserAccessPage>(), Is.EquivalentTo(["/settings/access/users/{UserId}"]));
            Assert.That(RouteTemplatesFor<EditRoleAccessPage>(), Is.EquivalentTo(["/settings/access/roles/{RoleId}"]));
            Assert.That(AuthorizationPoliciesFor<AccessSettingsPage>(), Has.One.EqualTo(Constants.Authorization.PolicyName));
            Assert.That(AuthorizationPoliciesFor<EditUserAccessPage>(), Has.One.EqualTo(Constants.Authorization.PolicyName));
            Assert.That(AuthorizationPoliciesFor<EditRoleAccessPage>(), Has.One.EqualTo(Constants.Authorization.PolicyName));
        });
    }

    private static ClaimsPrincipal PrincipalWithClaim()
    {
        return new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(AccessAuthorizationClaims.AccessClaimType, Constants.Authorization.AccessClaimValue)
        ], "test"));
    }

    private static ClaimsPrincipal PrincipalInAdminRole()
    {
        return new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(
                AccessAuthorizationClaims.ExternalDirectoryRoleType,
                AccessAuthorizationClaims.ApplicationAdminRoleClaimValue)
        ], "test"));
    }

    private static string[] RouteTemplatesFor<TComponent>()
    {
        return typeof(TComponent)
            .GetCustomAttributes(typeof(RouteAttribute), inherit: false)
            .Cast<RouteAttribute>()
            .Select(attribute => attribute.Template)
            .ToArray();
    }

    private static string?[] AuthorizationPoliciesFor<TComponent>()
    {
        return typeof(TComponent)
            .GetCustomAttributes(typeof(AuthorizeAttribute), inherit: false)
            .Cast<AuthorizeAttribute>()
            .Select(attribute => attribute.Policy)
            .ToArray();
    }
}

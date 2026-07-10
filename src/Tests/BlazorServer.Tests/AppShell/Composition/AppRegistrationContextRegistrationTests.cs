using BlazorServer.AppShell.Composition.UI;
using BlazorServer.AppShell.Composition.Navigation;
using BlazorServer.AppShell.Composition.Settings;
using BlazorServer.AppShell.Composition;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BlazorServer.Tests.AppShell.Composition;

[TestFixture]
public class AppRegistrationContextRegistrationTests
{
    [Test]
    public void ServicesCanCreateAndApplyAppRegistrationContext()
    {
        var services = new ServiceCollection();

        var context = services.AddAppRegistrationContext();
        context.Policies.AddPolicy("CanUseReports", policy => policy.RequireClaim("reports"));

        services.ApplyAppRegistrationContext(context);

        using var provider = services.BuildServiceProvider();
        var authorizationOptions = provider.GetRequiredService<IOptions<AuthorizationOptions>>().Value;

        Assert.Multiple(() =>
        {
            Assert.That(provider.GetRequiredService<AppRegistrationContext>(), Is.SameAs(context));
            Assert.That(provider.GetRequiredService<UiRegistry>(), Is.SameAs(context.Ui));
            Assert.That(provider.GetRequiredService<NavigationRegistry>(), Is.SameAs(context.Ui.Navigation));
            Assert.That(provider.GetRequiredService<SettingsAreaRegistry>(), Is.SameAs(context.Ui.SettingsAreaRegistry));
            Assert.That(provider.GetRequiredService<AccessClaimRegistry>(), Is.SameAs(context.AccessClaims));
            Assert.That(authorizationOptions.GetPolicy("CanUseReports"), Is.Not.Null);
        });
    }
}

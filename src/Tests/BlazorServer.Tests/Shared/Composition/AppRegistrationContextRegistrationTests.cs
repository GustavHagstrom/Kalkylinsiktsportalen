using BlazorServer.Shared.Composition;
using BlazorServer.Shared.FeatureRegistration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BlazorServer.Tests.Shared.Composition;

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
            Assert.That(provider.GetRequiredService<UiCompositionRegistry>(), Is.SameAs(context.UiComposition));
            Assert.That(provider.GetRequiredService<RenderCollection>(), Is.SameAs(context.RenderCollection));
            Assert.That(provider.GetRequiredService<ClaimInfoCollection>(), Is.SameAs(context.ClaimInfoCollection));
            Assert.That(authorizationOptions.GetPolicy("CanUseReports"), Is.Not.Null);
        });
    }
}

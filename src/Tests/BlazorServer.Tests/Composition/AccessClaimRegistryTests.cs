using BlazorServer.Modules.Access.Public.Contracts;
using BlazorServer.Composition;

namespace BlazorServer.Tests.Composition;

[TestFixture]
public class AccessClaimRegistryTests
{
    [Test]
    public void ClaimsAreExposedInRegistrationOrder()
    {
        var registry = new AccessClaimRegistry();
        var firstClaim = new AccessClaimContribution("FirstAccess", "First", "First claim.");
        var secondClaim = new AccessClaimContribution("SecondAccess", "Second", "Second claim.");

        registry.Register(firstClaim);
        registry.Register(secondClaim);

        Assert.That(registry.Claims, Is.EqualTo(new[] { firstClaim, secondClaim }));
    }

    [Test]
    public void IdenticalClaimRegistrationIsIdempotent()
    {
        var registry = new AccessClaimRegistry();
        var claim = new AccessClaimContribution("ApiSettingsAccess", "Bidcon API", "Change API credentials.");

        registry.Register(claim);
        registry.Register(new AccessClaimContribution("ApiSettingsAccess", "Bidcon API", "Change API credentials."));

        Assert.That(registry.Claims, Has.One.EqualTo(claim));
    }

    [Test]
    public void SameClaimValueWithDifferentMetadataFailsRegistration()
    {
        var registry = new AccessClaimRegistry();
        registry.Register(new AccessClaimContribution("ApiSettingsAccess", "Bidcon API", "Change API credentials."));

        var exception = Assert.Throws<InvalidOperationException>(() =>
            registry.Register(new AccessClaimContribution("ApiSettingsAccess", "API Admin", "Manage all Bidcon API access.")));

        Assert.That(exception!.Message, Does.Contain("ApiSettingsAccess").And.Contain("different metadata"));
    }
}

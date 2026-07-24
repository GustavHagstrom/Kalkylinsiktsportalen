using BlazorServer.Modules.Access.Public.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace BlazorServer.Tests.Modules.Access;

[TestFixture]
public class AccessAuthorizationPolicyBuilderExtensionsTests
{
    [Test]
    public void RequireAccessClaimOrApplicationAdminReturnsBuilderForChaining()
    {
        var builder = new AuthorizationPolicyBuilder();

        var returned = builder.RequireAccessClaimOrApplicationAdmin("CanDoThing");

        Assert.That(returned, Is.SameAs(builder));
    }

    [Test]
    public void RequireAccessClaimOrApplicationAdminRejectsMissingBuilder()
    {
        AuthorizationPolicyBuilder? builder = null;

        Assert.Throws<ArgumentNullException>(() =>
            builder!.RequireAccessClaimOrApplicationAdmin("CanDoThing"));
    }

    [Test]
    public void RequireAccessClaimOrApplicationAdminRejectsMissingClaimValue()
    {
        var builder = new AuthorizationPolicyBuilder();

        Assert.Throws<ArgumentException>(() =>
            builder.RequireAccessClaimOrApplicationAdmin(" "));
    }
}

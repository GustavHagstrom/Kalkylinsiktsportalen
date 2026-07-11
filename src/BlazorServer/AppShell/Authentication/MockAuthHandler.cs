using BlazorServer.Shared.MockedData;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace BlazorServer.AppShell.Authentication;

public class MockAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly MockedAuthSession _session;

    public MockAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        MockedAuthSession session) : base(options, logger, encoder)
    {
        _session = session;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var user = _session.User;
        if (user is null)
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var roles = MockedDataStore.Roles
            .Where(role => role.Id == user.Id)
            .Select(role => new Claim("groups", role.Id));

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Name),
            new(ClaimConstants.ObjectId, user.Id),
            new(ClaimConstants.TenantId, MockedDataStore.OrganizationId),
            new(CurrentUserSessionClaimTypes.Email, "gustav@rorvikshus.se")
        };
        claims.AddRange(roles);

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}

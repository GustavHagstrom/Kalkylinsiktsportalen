using BlazorServer.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace BlazorServer.AppShell.Authentication.Mock;

public class MockAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly MockCurrentUserSession _session;
    private readonly MockAuthDefaults _defaults;

    public MockAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        MockCurrentUserSession session,
        MockAuthDefaults defaults) : base(options, logger, encoder)
    {
        _session = session;
        _defaults = defaults;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var user = _session.CurrentUser;
        if (user is null)
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var roles = _defaults.RoleIdsByUserId.TryGetValue(user.UserId.Value, out var roleIds)
            ? roleIds.Select(roleId => new Claim(IdentityClaimTypes.Groups, roleId))
            : [];

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.DisplayName),
            new(IdentityClaimTypes.UserId, user.UserId.Value),
            new(IdentityClaimTypes.OrganizationId, _defaults.OrganizationId),
            new(IdentityClaimTypes.Email, user.Email ?? string.Empty)
        };
        claims.AddRange(roles);

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}

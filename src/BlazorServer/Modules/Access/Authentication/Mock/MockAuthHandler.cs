using BlazorServer.AppShell.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace BlazorServer.Modules.Access.Authentication.Mock;

public class MockAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly MockedAuthSession _session;
    private readonly MockAuthDefaults _defaults;

    public MockAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        MockedAuthSession session,
        MockAuthDefaults defaults) : base(options, logger, encoder)
    {
        _session = session;
        _defaults = defaults;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var user = _session.User;
        if (user is null)
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var roles = _defaults.RoleIdsByUserId.TryGetValue(user.Id, out var roleIds)
            ? roleIds.Select(roleId => new Claim("groups", roleId))
            : [];

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Name),
            new(ClaimConstants.ObjectId, user.Id),
            new(ClaimConstants.TenantId, _defaults.OrganizationId),
            new(CurrentUserSessionClaimTypes.Email, user.Email ?? string.Empty)
        };
        if(user.Id.ToLower() == "admin")
        {
            claims.Add(new Claim(ApplicationClaimConstants.ExternalDirectoryRoleType, ApplicationClaimConstants.ApplicationAdminRoleClaimValue));
        }
        claims.AddRange(roles);

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}

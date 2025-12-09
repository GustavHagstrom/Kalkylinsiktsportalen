using BlazorServer.Shared.Constants;
using BlazorServer.Shared.MockedData;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace BlazorServer.Shared.Services.Authentication;

public class MockAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly MockedAuthSession _session;

    public MockAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, MockedAuthSession session) : base(options, logger, encoder)
    {
        _session = session;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var user = _session.User;
        if (user == null) return Task.FromResult(AuthenticateResult.NoResult());

        var roles = MockedDataStore.Roles.Where(r => r.Id == user.Id).Select(r => new Claim("groups", r.Id));

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimConstants.ObjectId, user.Id),
            new Claim(ClaimConstants.TenantId, MockedDataStore.OrganizationId),
            new Claim(@"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", "gustav@rorvikshus.se")
        };
        claims.AddRange(roles);

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}


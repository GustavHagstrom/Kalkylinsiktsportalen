using BlazorServer.Shared.Constants;
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

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimConstants.ObjectId, user.Id),
            new Claim(ClaimConstants.TenantId, "OrganizationID123"),
        };
       return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(new ClaimsIdentity(claims, Scheme.Name)), Scheme.Name)));
    }
}


using BlazorServer.Shared.Constants;
using BlazorServer.Shared.MockedData;
using BlazorServer.Shared.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BlazorServer.Shared.Services.Authentication;

public class MockedAuthStateProvider : AuthenticationStateProvider
{
    private bool _isLoggedIn = true;
    private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

    private ClaimsPrincipal SetUser(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(AuthConstants.ObjectId, user.Id),
            new Claim(AuthConstants.TenantId, "OrganizationID123"),
        };
        if(user.Name == "Admin") claims.Add(new Claim(ClaimTypes.Role, AuthConstants.AdminRoleClaimValue));
        _user = new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));
        return _user;
    }
    private ClaimsPrincipal? _user;
    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (_isLoggedIn && _user is not null)
        {
            return Task.FromResult(new AuthenticationState(_user));
        }
        // If not logged in, return an anonymous user
        return Task.FromResult(new AuthenticationState(_anonymous));
    }
    public void SignIn()
    {
        _isLoggedIn = true;
        SetUser(MockedDataStore.Users.First());
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
    public void SignIn(User user)
    {
        _isLoggedIn = true;
        SetUser(user);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
    public void SignOut()
    {
        _isLoggedIn = false;
        _user = null;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}

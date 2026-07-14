using BlazorServer.AppShell.Routes;
using BlazorServer.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BlazorServer.AppShell.Authentication;

public class MockCurrentUserSession : ICurrentUserContext, IAuthService
{

    private readonly MockAuthDefaults _defaults;
    private readonly NavigationManager _navigation;
    private readonly AuthenticationStateProvider _stateProvider;

    public MockCurrentUserSession(
        MockAuthDefaults defaults,
        NavigationManager navigation,
        AuthenticationStateProvider stateProvider)
    {
        _defaults = defaults;
        _navigation = navigation;
        _stateProvider = stateProvider;
    }


    public CurrentUser? CurrentUser { get; private set; }

    public IReadOnlyList<Claim> Claims => _stateProvider.GetAuthenticationStateAsync().Result.User.Claims.ToList();

    public Task LoginAsync()
    {
        CurrentUser = _defaults.Users.First();
        _navigation.NavigateTo(AppShellRoutes.Home, forceLoad: true);
        return Task.CompletedTask;
    }

    public void Login(CurrentUser user)
    {
        CurrentUser = user;
        _navigation.Refresh(forceReload: true);
    }

    public Task LogoutAsync()
    {
        CurrentUser = null;
        _navigation.Refresh(forceReload: true);
        return Task.CompletedTask;
    }

    public Task<CurrentUser?> RefreshAsync()
    {
        return Task.FromResult(CurrentUser);
    }


}

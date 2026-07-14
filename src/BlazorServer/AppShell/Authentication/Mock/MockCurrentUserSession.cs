using BlazorServer.AppShell.Routes;
using BlazorServer.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BlazorServer.AppShell.Authentication.Mock;

public class MockCurrentUserSession : ICurrentUserContext, IAuthService
{

    private readonly MockAuthDefaults _defaults;
    private readonly NavigationManager _navigation;
    private readonly AuthenticationStateProvider _stateProvider;
    private readonly MockAuthenticationState _mockAuthenticationState;

    public MockCurrentUserSession(
        MockAuthDefaults defaults,
        NavigationManager navigation,
        AuthenticationStateProvider stateProvider,
        MockAuthenticationState mockAuthenticationState)
    {
        _defaults = defaults;
        _navigation = navigation;
        _stateProvider = stateProvider;
        _mockAuthenticationState = mockAuthenticationState;
    }


    public CurrentUser? CurrentUser => _mockAuthenticationState.CurrentUser;

    public IReadOnlyList<Claim> Claims => _stateProvider.GetAuthenticationStateAsync().Result.User.Claims.ToList();

    public Task LoginAsync()
    {
        _mockAuthenticationState.CurrentUser = _defaults.Users.First();
        _navigation.NavigateTo(AppShellRoutes.Home, forceLoad: true);
        return Task.CompletedTask;
    }

    public void Login(CurrentUser user)
    {
        _mockAuthenticationState.CurrentUser = user;
        _navigation.Refresh(forceReload: true);
    }

    public Task LogoutAsync()
    {
        _mockAuthenticationState.CurrentUser = null;
        _navigation.Refresh(forceReload: true);
        return Task.CompletedTask;
    }

    public Task<CurrentUser?> RefreshAsync()
    {
        return Task.FromResult(CurrentUser);
    }


}

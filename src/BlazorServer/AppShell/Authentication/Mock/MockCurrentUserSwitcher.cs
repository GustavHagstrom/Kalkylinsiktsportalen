using BlazorServer.Identity;
using Microsoft.AspNetCore.Components;

namespace BlazorServer.AppShell.Authentication.Mock;

public sealed class MockCurrentUserSwitcher
{
    private readonly MockAuthenticationState authenticationState;
    private readonly NavigationManager navigation;

    public MockCurrentUserSwitcher(
        MockAuthenticationState authenticationState,
        NavigationManager navigation)
    {
        this.authenticationState = authenticationState;
        this.navigation = navigation;
    }

    public void Login(CurrentUser user)
    {
        authenticationState.CurrentUser = user;
        navigation.Refresh(forceReload: true);
    }

    public void Logout()
    {
        authenticationState.CurrentUser = null;
        navigation.Refresh(forceReload: true);
    }
}

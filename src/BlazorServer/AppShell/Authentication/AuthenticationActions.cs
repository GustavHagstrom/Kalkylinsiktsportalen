using Microsoft.AspNetCore.Components;

namespace BlazorServer.AppShell.Authentication;

public sealed class AuthenticationActions : IAuthenticationActions
{
    private readonly NavigationManager navigation;

    public AuthenticationActions(NavigationManager navigation)
    {
        this.navigation = navigation;
    }

    public void Login()
    {
        navigation.NavigateTo("/auth/login", forceLoad: true);
    }

    public void Logout()
    {
        navigation.NavigateTo("/auth/logout", forceLoad: true);
    }
}

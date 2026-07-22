using BlazorServer.Identity;

namespace BlazorServer.AppShell.Authentication.Mock;

public sealed class MockAuthenticationState
{
    public CurrentUser? CurrentUser { get; set; }
}

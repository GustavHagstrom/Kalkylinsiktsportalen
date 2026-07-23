using BlazorServer.Identity;

namespace BlazorServer.AppShell.Authentication.Mock;

public sealed class MockCurrentUserContext : ICurrentUserContext
{
    private readonly MockAuthenticationState authenticationState;

    public MockCurrentUserContext(MockAuthenticationState authenticationState)
    {
        this.authenticationState = authenticationState;
    }

    public ValueTask<CurrentUser?> GetCurrentUserAsync(CancellationToken cancellationToken = default)
        => ValueTask.FromResult(authenticationState.CurrentUser);
}

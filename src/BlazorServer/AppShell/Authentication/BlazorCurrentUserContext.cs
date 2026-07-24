using BlazorServer.Identity;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BlazorServer.AppShell.Authentication;

public sealed class BlazorCurrentUserContext : ICurrentUserContext
{

    private readonly AuthenticationStateProvider authProvider;

    public BlazorCurrentUserContext(AuthenticationStateProvider authProvider)
    {
        this.authProvider = authProvider;
    }

    public async Task<CurrentUser?> GetCurrentUserAsync(CancellationToken cancellationToken = default)
    {
        var currentState = await authProvider.GetAuthenticationStateAsync();
        return currentState.User.ToCurrentUser();
    }
}

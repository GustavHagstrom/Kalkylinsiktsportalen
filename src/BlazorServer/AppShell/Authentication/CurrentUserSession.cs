using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BlazorServer.AppShell.Authentication;

public class CurrentUserSession : ICurrentUserSession, IAuthService, IDisposable
{
    private readonly AuthenticationStateProvider _authProvider;
    private readonly NavigationManager _navigationManager;
    private AuthenticationState? _cachedAuthState;

    public CurrentUserSession(
        AuthenticationStateProvider authProvider,
        NavigationManager navigationManager)
    {
        _authProvider = authProvider;
        _navigationManager = navigationManager;
        _authProvider.AuthenticationStateChanged += OnAuthenticationStateChanged;
    }

    public string? OrganizationId => _cachedAuthState?.User.GetOrganizationId();

    public string? UserEmail => CurrentUser?.Email;

    public bool IsAuthenticated => _cachedAuthState?.User.Identity?.IsAuthenticated ?? false;

    public CurrentUser? CurrentUser => _cachedAuthState?.User.ToCurrentUser();

    public SessionUser? User => CurrentUser is null
        ? null
        : new SessionUser(CurrentUser.UserId, CurrentUser.DisplayName, CurrentUser.Email, null);

    public IReadOnlyList<Claim> Claims => _cachedAuthState?.User.Claims.ToList() ?? new List<Claim>();

    public void Dispose()
    {
        _authProvider.AuthenticationStateChanged -= OnAuthenticationStateChanged;
    }

    public Task LoginAsync()
    {
        _navigationManager.NavigateTo("/auth/login", true);
        return Task.CompletedTask;
    }

    public Task LogoutAsync()
    {
        _navigationManager.NavigateTo("/auth/logout", true);
        return Task.CompletedTask;
    }

    public async Task<CurrentUser?> RefreshAsync()
    {
        await UpdateCachedStateAsync(_authProvider.GetAuthenticationStateAsync());
        return CurrentUser;
    }

    public async Task<SessionUser?> UpdateUserAsync()
    {
        await RefreshAsync();
        return User;
    }

    private void OnAuthenticationStateChanged(Task<AuthenticationState> task)
    {
        _ = UpdateCachedStateAsync(task);
    }

    private async Task UpdateCachedStateAsync(Task<AuthenticationState> task)
    {
        _cachedAuthState = await task;
        if (!IsAuthenticated)
        {
            return;
        }
    }
}

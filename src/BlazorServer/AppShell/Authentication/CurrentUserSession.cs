using BlazorServer.Shared.Models;
using BlazorServer.Shared.Services.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BlazorServer.AppShell.Authentication;

public class CurrentUserSession : ICurrentUserSession, IAuthService, IDisposable
{
    private readonly AuthenticationStateProvider _authProvider;
    private readonly NavigationManager _navigationManager;
    private readonly IUserService _userService;
    private AuthenticationState? _cachedAuthState;
    private User? _cachedUser;

    public CurrentUserSession(
        AuthenticationStateProvider authProvider,
        NavigationManager navigationManager,
        IUserService userService)
    {
        _authProvider = authProvider;
        _navigationManager = navigationManager;
        _userService = userService;
        _authProvider.AuthenticationStateChanged += OnAuthenticationStateChanged;
    }

    public string? OrganizationId => _cachedAuthState?.User.GetOrganizationId();

    public string? UserEmail => CurrentUser?.Email;

    public bool IsAuthenticated => _cachedAuthState?.User.Identity?.IsAuthenticated ?? false;

    public CurrentUser? CurrentUser => _cachedAuthState?.User.ToCurrentUser();

    public User? User => _cachedUser;

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

    public async Task<User?> UpdateUserAsync()
    {
        await RefreshAsync();
        return _cachedUser;
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
            _cachedUser = null;
            return;
        }

        var userId = CurrentUser?.UserId;
        _cachedUser = userId is null ? null : await _userService.GetUserAsync(userId);
    }
}

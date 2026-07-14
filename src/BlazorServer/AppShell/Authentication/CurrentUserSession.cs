using BlazorServer.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BlazorServer.AppShell.Authentication;

public class CurrentUserSession : ICurrentUserContext, IAuthService, IDisposable
{
    private readonly AuthenticationStateProvider _authProvider;
    private readonly NavigationManager _navigationManager;
    private readonly ICurrentUserProfileLookup _profileLookup;
    private AuthenticationState? _cachedAuthState;

    public CurrentUserSession(
        AuthenticationStateProvider authProvider,
        NavigationManager navigationManager,
        ICurrentUserProfileLookup profileLookup)
    {
        _authProvider = authProvider;
        _navigationManager = navigationManager;
        _profileLookup = profileLookup;
        _authProvider.AuthenticationStateChanged += OnAuthenticationStateChanged;
    }


    public CurrentUser? CurrentUser { get; private set; }

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

    private void OnAuthenticationStateChanged(Task<AuthenticationState> task)
    {
        _ = UpdateCachedStateAsync(task);
    }

    private async Task UpdateCachedStateAsync(Task<AuthenticationState> task)
    {
        _cachedAuthState = await task;
        if(_cachedAuthState.User.Identity?.IsAuthenticated != true)
        {
            CurrentUser = null;
            return;
        }
        CurrentUser = _cachedAuthState.User.ToCurrentUser();
        if (CurrentUser != null && CurrentUser.Avatar == null)
        {
            CurrentUser = await _profileLookup.GetProfileAsync(CurrentUser);
        }

    }
}
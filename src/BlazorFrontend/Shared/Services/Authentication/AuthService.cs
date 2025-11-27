using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.Authorization;


namespace BlazorFrontend.Shared.Services.Authentication;

public class AuthService : IAuthService, IDisposable
{
    private readonly AuthenticationStateProvider _authProvider;
    private readonly NavigationManager _navigationManager;

    private AuthenticationState? _cachedAuthState;

    public AuthService(AuthenticationStateProvider authProvider, NavigationManager navigationManager)
    {
        _authProvider = authProvider;
        _navigationManager = navigationManager;
        _authProvider.AuthenticationStateChanged += OnAuthenticationStateChanged;
    }

    private void OnAuthenticationStateChanged(Task<AuthenticationState> task)
    {
        _ = UpdateCachedStateAsync(task);
    }
    async Task UpdateCachedStateAsync(Task<AuthenticationState> task)
    {
        try
        {
            _cachedAuthState = await task;
        }
        catch (Exception e)
        {
            //should probably logg?
            throw;
        }
    }

    public void Dispose()
    {
        _authProvider.AuthenticationStateChanged -= OnAuthenticationStateChanged;
    }
    public string? GetOrganizationId()
    {
        var OrgId = _cachedAuthState?.User.FindFirst(AuthConstants.TenantId)?.Value;
        return OrgId;
    }

    public string? GetUserId()
    {
        var userId = _cachedAuthState?.User.FindFirst(AuthConstants.ObjectId)?.Value;
        return userId;
    }

    public string? GetUserName()
    {
        var name = _cachedAuthState?.User.Identity?.Name;
        return name;
    }

    public bool IsUserAuthenticated()
    {
        var isAuthenticated = _cachedAuthState?.User.Identity?.IsAuthenticated ?? false;
        return isAuthenticated;
    }

    public async Task LoginAsync()
    {
        _navigationManager.NavigateToLogin("authentication/login");
    }

    public async Task LogoutAsync()
    {
        _navigationManager.NavigateToLogout("authentication/logout");
    }
}

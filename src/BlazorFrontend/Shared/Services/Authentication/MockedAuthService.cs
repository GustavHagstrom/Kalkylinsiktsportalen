using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorFrontend.Shared.Services.Authentication;

public class MockedAuthService : IAuthService
{
    private readonly AuthenticationStateProvider _authProvider;

    private AuthenticationState? _cachedAuthState;
    public MockedAuthService(AuthenticationStateProvider authProvider)
    {
        authProvider.AuthenticationStateChanged += OnAuthenticationStateChanged;
        _authProvider = authProvider;
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
    public Task LoginAsync()
    {
        if (_authProvider is MockedAuthenticationStateProvider mockedAuthStateProvider)
        {
            mockedAuthStateProvider.SignIn();
        }
        
        return Task.CompletedTask;
    }
    public Task LogoutAsync()
    {
        if (_authProvider is MockedAuthenticationStateProvider mockedAuthStateProvider)
        {
            mockedAuthStateProvider.SignOut();
        }
        return Task.CompletedTask;
    }
}
 
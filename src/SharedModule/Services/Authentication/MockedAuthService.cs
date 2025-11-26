using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Identity.Web;

namespace SharedModule.Services.Authentication;

public class MockedAuthService : IAuthService
{
    private readonly AuthenticationStateProvider _authProvider;
    private readonly NavigationManager _navigationManager;


    private AuthenticationState? _cachedAuthState;
    public MockedAuthService(AuthenticationStateProvider authProvider, NavigationManager navigationManager)
    {
        authProvider.AuthenticationStateChanged += OnAuthenticationStateChanged;
        _authProvider = authProvider;
        _navigationManager = navigationManager;
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
        var OrgId = _cachedAuthState?.User.FindFirst(ClaimConstants.TenantId)?.Value;
        return OrgId;
    }

    public string? GetUserId()
    {
        var userId = _cachedAuthState?.User.FindFirst(ClaimConstants.ObjectId)?.Value;
        return userId;
    }

    public string? GetUserName()
    {
        var name = _cachedAuthState?.User.GetDisplayName();
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
        //_navigationManager.NavigateTo("/");
        
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

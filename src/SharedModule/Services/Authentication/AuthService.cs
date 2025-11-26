
using Core.Constants;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Identity.Web;
using Microsoft.JSInterop;

namespace SharedModule.Services.Authentication;

public class AuthService : IAuthService, IDisposable
{
    private readonly AuthenticationStateProvider _authProvider;
    private readonly ILocalStorageService _localStorageService;
    private readonly NavigationManager _navigationManager;
    private readonly IJSRuntime _jSRuntime;

    private AuthenticationState? _cachedAuthState;

    public AuthService(AuthenticationStateProvider authProvider, ILocalStorageService localStorageService, NavigationManager navigationManager, IJSRuntime jSRuntime)
    {
        _authProvider = authProvider;
        _localStorageService = localStorageService;
        _navigationManager = navigationManager;
        _jSRuntime = jSRuntime;
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

    public async Task LoginAsync()
    {
        await _localStorageService.SetItemAsync(LocalStorageKeys.RedirectPostLoginUrl, _navigationManager.Uri);
        var loginUrl = _navigationManager.ToAbsoluteUri($"/MicrosoftIdentity/Account/SignIn").ToString();
        _navigationManager.NavigateTo(loginUrl, forceLoad: true);
        //await _jSRuntime.InvokeVoidAsync("eval", $"window.location.replace('{loginUrl}');");
    }

    public async Task LogoutAsync()
    {
        var logoutUrl = _navigationManager.ToAbsoluteUri($"/MicrosoftIdentity/Account/SignOut").ToString();
        _navigationManager.NavigateTo(logoutUrl, forceLoad: true);
        //await _jSRuntime.InvokeVoidAsync("eval", $"window.location.replace('{logoutUrl}');");
    }
}

using BlazorServer.Shared.Constants;
using BlazorServer.Shared.Extensions;
using BlazorServer.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;


namespace BlazorServer.Shared.Services.Authentication;

public class AuthService : IAuthService, IDisposable
{
    private readonly AuthenticationStateProvider _authProvider;
    private readonly NavigationManager _navigationManager;
    private readonly IUserService _userService;
    private AuthenticationState? _cachedAuthState;
    private User? _cachedUser;

    public AuthService(AuthenticationStateProvider authProvider, NavigationManager navigationManager, IUserService userService) 
    {
        _authProvider = authProvider;
        _navigationManager = navigationManager;
        _userService = userService;
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
            if (IsAuthenticated)
            {
                var userId = UserId;
                if (userId is not null)
                {
                    _cachedUser = await _userService.GetUserAsync(userId);
                }
            }
            else
            {
                _cachedUser = null;
            }
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
    public string? OrganizationId => _cachedAuthState?.User.GetOrganizationId();

    public string? UserId => _cachedAuthState?.User.GetUserId();

    public bool IsAuthenticated => _cachedAuthState?.User.Identity?.IsAuthenticated ?? false;

    public User? User => _cachedUser;

    public string? UserEmail => _cachedAuthState?.User.GetUserEmail();

    public async Task LoginAsync()
    {
        _navigationManager.NavigateTo($"/auth/login", true);
    }

    public async Task LogoutAsync()
    {
        _navigationManager.NavigateTo($"/auth/logout", true);
    }

    public async Task<User?> UpdateUserAsync()
    {
        await UpdateCachedStateAsync(_authProvider.GetAuthenticationStateAsync());
        return _cachedUser;
    }
}

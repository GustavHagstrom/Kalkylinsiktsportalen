using BlazorServer.Shared.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorServer.Shared.Services.Authentication;

public class MockedAuthService : IAuthService, IDisposable
{
    private readonly AuthenticationStateProvider _authProvider;
    private readonly IUserService _userService;

    private AuthenticationState? _cachedAuthState;
    private User? _cachedUser;
    public MockedAuthService(AuthenticationStateProvider authProvider, IUserService userService)
    {
        authProvider.AuthenticationStateChanged += OnAuthenticationStateChanged;
        _authProvider = authProvider;
        _userService = userService;
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
    public string? OrganizationId => _cachedAuthState?.User.FindFirst(AuthConstants.TenantId)?.Value;

    public string? UserId => _cachedAuthState?.User.FindFirst(AuthConstants.ObjectId)?.Value;

    public bool IsAuthenticated => _cachedAuthState?.User.Identity?.IsAuthenticated ?? false;

    public User? User => _cachedUser;
    public Task LoginAsync()
    {
        if (_authProvider is MockedAuthStateProvider mockedAuthStateProvider)
        {
            mockedAuthStateProvider.SignIn();
        }
        
        return Task.CompletedTask;
    }
    public void Login(User user)
    {
        if (_authProvider is MockedAuthStateProvider mockedAuthStateProvider)
        {
            mockedAuthStateProvider.SignIn(user);
        }
    }
    public Task LogoutAsync()
    {
        if (_authProvider is MockedAuthStateProvider mockedAuthStateProvider)
        {
            mockedAuthStateProvider.SignOut();
        }
        return Task.CompletedTask;
    }
}
 
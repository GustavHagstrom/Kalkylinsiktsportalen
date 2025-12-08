using BlazorServer.Shared.Constants;
using BlazorServer.Shared.Extensions;
using BlazorServer.Shared.MockedData;
using BlazorServer.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorServer.Shared.Services.Authentication;

public class MockedAuthService : IAuthService, IDisposable
{
    private readonly AuthenticationStateProvider _authProvider;
    private readonly IUserService _userService;
    private readonly MockedAuthSession _session;
    private readonly NavigationManager _navigation;
    private AuthenticationState? _cachedAuthState;
    private User? _cachedUser;
    public MockedAuthService(AuthenticationStateProvider authProvider, IUserService userService, MockedAuthSession session, NavigationManager navigation)
    {
        authProvider.AuthenticationStateChanged += OnAuthenticationStateChanged;
        _authProvider = authProvider;
        _userService = userService;
        _session = session;
        _navigation = navigation;
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
    public Task LoginAsync()
    {
        _session.CurrentUser = MockedDataStore.Users.FirstOrDefault();
        _navigation.NavigateTo(AppRoutes.Home, forceLoad: true);
        return Task.CompletedTask;
    }
    public void Login(User user)
    {
        _session.CurrentUser = user;
        _navigation.NavigateTo(AppRoutes.Home, forceLoad: true);

        //if (_authProvider is MockedAuthStateProvider mockedAuthStateProvider)
        //{
        //    mockedAuthStateProvider.SignIn(user);
        //}
    }
    public Task LogoutAsync()
    {
        _session.CurrentUser = null;
        _navigation.NavigateTo(AppRoutes.Home, forceLoad: true);

        //if (_authProvider is MockedAuthStateProvider mockedAuthStateProvider)
        //{
        //    mockedAuthStateProvider.SignOut();
        //}
        return Task.CompletedTask;
    }
    public async Task<User?> UpdateUserAsync()
    {
        await UpdateCachedStateAsync(_authProvider.GetAuthenticationStateAsync());
        return _cachedUser;
    }
}
 
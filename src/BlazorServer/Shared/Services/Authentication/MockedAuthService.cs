using BlazorServer.Shared.Constants;
using BlazorServer.Shared.MockedData;
using BlazorServer.Shared.Models;
using Microsoft.AspNetCore.Components;

namespace BlazorServer.Shared.Services.Authentication;

public class MockedAuthService : IAuthService
{
    private readonly MockedAuthSession _session;
    private readonly NavigationManager _navigation;
    public MockedAuthService(IUserService userService, MockedAuthSession session, NavigationManager navigation)
    {
        _session = session;
        _navigation = navigation;
    }
    public string? OrganizationId => _session.OrganizationId;
    public string? UserId => _session.User?.Id;
    public bool IsAuthenticated => _session.User is not null;
    public User? User => _session.User;
    public Task LoginAsync()
    {
        _session.SetSession(MockedDataStore.Users.First(), "mockedOrgId");
        _navigation.NavigateTo(AppRoutes.Home, forceLoad: true);
        return Task.CompletedTask;
    }
    public void Login(User user)
    {
        _session.SetSession(user, "mockedOrgId");
        _navigation.Refresh(forceReload: true);
    }
    public Task LogoutAsync()
    {
        _session.ClearSession();
        _navigation.Refresh(forceReload: true);
        return Task.CompletedTask;
    }
    public async Task<User?> UpdateUserAsync()
    {
        return User;
    }
}
 
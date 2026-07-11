using BlazorServer.AppShell.Routes;
using BlazorServer.Shared.MockedData;
using BlazorServer.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BlazorServer.AppShell.Authentication;

public class MockCurrentUserSession : ICurrentUserSession, IAuthService
{
    private readonly MockedAuthSession _session;
    private readonly NavigationManager _navigation;
    private readonly AuthenticationStateProvider _stateProvider;

    public MockCurrentUserSession(
        MockedAuthSession session,
        NavigationManager navigation,
        AuthenticationStateProvider stateProvider)
    {
        _session = session;
        _navigation = navigation;
        _stateProvider = stateProvider;
    }

    public string? OrganizationId => _session.OrganizationId;

    public bool IsAuthenticated => _session.User is not null;

    public CurrentUser? CurrentUser => _session.User is null
        ? null
        : new CurrentUser(_session.User.Id, _session.User.Name, UserEmail);

    public User? User => _session.User;

    public string? UserEmail => "gustav@rorvikshus.se";

    public IReadOnlyList<Claim> Claims => _stateProvider.GetAuthenticationStateAsync().Result.User.Claims.ToList();

    public Task LoginAsync()
    {
        _session.SetSession(MockedDataStore.Users.First(), MockedDataStore.OrganizationId);
        _navigation.NavigateTo(AppShellRoutes.Home, forceLoad: true);
        return Task.CompletedTask;
    }

    public void Login(User user)
    {
        _session.SetSession(user, MockedDataStore.OrganizationId);
        _navigation.Refresh(forceReload: true);
    }

    public Task LogoutAsync()
    {
        _session.ClearSession();
        _navigation.Refresh(forceReload: true);
        return Task.CompletedTask;
    }

    public Task<CurrentUser?> RefreshAsync()
    {
        return Task.FromResult(CurrentUser);
    }

    public Task<User?> UpdateUserAsync()
    {
        return Task.FromResult(User);
    }
}

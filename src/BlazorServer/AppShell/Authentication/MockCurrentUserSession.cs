using BlazorServer.AppShell.Routes;
using BlazorServer.Modules.Access.Authentication.Mock;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BlazorServer.AppShell.Authentication;

public class MockCurrentUserSession : ICurrentUserSession, IAuthService
{
    private readonly MockedAuthSession _session;
    private readonly MockAuthDefaults _defaults;
    private readonly NavigationManager _navigation;
    private readonly AuthenticationStateProvider _stateProvider;

    public MockCurrentUserSession(
        MockedAuthSession session,
        MockAuthDefaults defaults,
        NavigationManager navigation,
        AuthenticationStateProvider stateProvider)
    {
        _session = session;
        _defaults = defaults;
        _navigation = navigation;
        _stateProvider = stateProvider;
    }

    public string? OrganizationId => _session.OrganizationId;

    public bool IsAuthenticated => _session.User is not null;

    public CurrentUser? CurrentUser => _session.User is null
        ? null
        : new CurrentUser(_session.User.Id, _session.User.Name, _session.User.Email);

    public SessionUser? User => _session.User;

    public string? UserEmail => _session.User?.Email;

    public IReadOnlyList<Claim> Claims => _stateProvider.GetAuthenticationStateAsync().Result.User.Claims.ToList();

    public Task LoginAsync()
    {
        _session.SetSession(_defaults.Users.First(), _defaults.OrganizationId);
        _navigation.NavigateTo(AppShellRoutes.Home, forceLoad: true);
        return Task.CompletedTask;
    }

    public void Login(SessionUser user)
    {
        _session.SetSession(user, _defaults.OrganizationId);
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

    public Task<SessionUser?> UpdateUserAsync()
    {
        return Task.FromResult(User);
    }
}

using BlazorServer.Modules.Access.Public.Authorization;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorServer.AppShell.Authentication;

public sealed class CurrentAccessSubject(
    AuthenticationStateProvider authenticationStateProvider,
    IAccessSubjectFactory accessSubjectFactory)
    : ICurrentAccessSubject
{
    public async Task<AccessSubject?> GetAsync(CancellationToken cancellationToken = default)
    {
        var authenticationState = await authenticationStateProvider.GetAuthenticationStateAsync();
        return accessSubjectFactory.Create(authenticationState.User);
    }
}

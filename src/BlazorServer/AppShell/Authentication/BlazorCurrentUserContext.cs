using BlazorServer.Identity;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BlazorServer.AppShell.Authentication;

public sealed class BlazorCurrentUserContext : ICurrentUserContext, IDisposable
{
    private static readonly ClaimsPrincipal AnonymousPrincipal = new(new ClaimsIdentity());

    private readonly AuthenticationStateProvider authProvider;
    private AuthenticationState currentState = new(AnonymousPrincipal);
    private Task<AuthenticationState> currentStateTask;

    public BlazorCurrentUserContext(AuthenticationStateProvider authProvider)
    {
        this.authProvider = authProvider;
        authProvider.AuthenticationStateChanged += OnAuthenticationStateChanged;

        currentStateTask = authProvider.GetAuthenticationStateAsync();
        if (currentStateTask.IsCompletedSuccessfully)
        {
            currentState = currentStateTask.Result;
        }
        else
        {
            _ = UpdateCurrentStateAsync(currentStateTask);
        }
    }

    public async ValueTask<CurrentUser?> GetCurrentUserAsync(CancellationToken cancellationToken = default)
    {
        currentState = await currentStateTask.WaitAsync(cancellationToken);
        return currentState.User.ToCurrentUser();
    }

    public void Dispose()
    {
        authProvider.AuthenticationStateChanged -= OnAuthenticationStateChanged;
    }

    private void OnAuthenticationStateChanged(Task<AuthenticationState> task)
    {
        currentStateTask = task;
        _ = UpdateCurrentStateAsync(task);
    }

    private async Task UpdateCurrentStateAsync(Task<AuthenticationState> task)
    {
        currentState = await task;
    }
}

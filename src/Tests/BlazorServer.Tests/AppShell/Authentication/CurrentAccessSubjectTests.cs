using BlazorServer.AppShell.Authentication;
using BlazorServer.Modules.Access.Public.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BlazorServer.Tests.AppShell.Authentication;

[TestFixture]
public class CurrentAccessSubjectTests
{
    [Test]
    public void CurrentAccessSubjectObservesCancellationBeforeReadingAuthenticationState()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();
        var currentSubject = new CurrentAccessSubject(
            new ThrowingAuthenticationStateProvider(),
            new ThrowingAccessSubjectFactory());

        Assert.ThrowsAsync<OperationCanceledException>(() =>
            currentSubject.GetAsync(cancellationTokenSource.Token));
    }

    private sealed class ThrowingAuthenticationStateProvider : AuthenticationStateProvider
    {
        public override Task<AuthenticationState> GetAuthenticationStateAsync()
            => throw new AssertionException("Authentication state should not be read after cancellation.");
    }

    private sealed class ThrowingAccessSubjectFactory : IAccessSubjectFactory
    {
        public AccessSubject? Create(ClaimsPrincipal principal)
            => throw new AssertionException("Access subject should not be created after cancellation.");
    }
}

using System.Security.Claims;

namespace BlazorServer.AppShell.Authentication;

public interface ICurrentUserSession
{
    bool IsAuthenticated { get; }
    CurrentUser? CurrentUser { get; }
    string? OrganizationId { get; }
    Task<CurrentUser?> RefreshAsync();
    IReadOnlyList<Claim> Claims { get; }
}

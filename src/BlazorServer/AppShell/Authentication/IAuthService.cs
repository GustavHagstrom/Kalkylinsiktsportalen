namespace BlazorServer.AppShell.Authentication;

public interface IAuthService
{
    Task LoginAsync();
    Task LogoutAsync();
    bool IsAuthenticated { get; }
    SessionUser? User { get; }
    string? OrganizationId { get; }
    string? UserEmail { get; }
    Task<SessionUser?> UpdateUserAsync();
}

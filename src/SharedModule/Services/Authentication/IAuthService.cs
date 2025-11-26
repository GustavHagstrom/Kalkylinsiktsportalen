namespace SharedModule.Services.Authentication;

public interface IAuthService
{
    Task LoginAsync();
    Task LogoutAsync();
    bool IsUserAuthenticated();
    string? GetUserId();
    string? GetOrganizationId();
    string? GetUserName();
}

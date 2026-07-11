namespace BlazorServer.AppShell.Authentication;

public sealed record CurrentUser(
    string UserId,
    string DisplayName,
    string? Email);

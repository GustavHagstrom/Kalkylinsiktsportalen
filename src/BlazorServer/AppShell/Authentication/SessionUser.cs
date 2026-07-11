namespace BlazorServer.AppShell.Authentication;

public sealed record SessionUser(
    string Id,
    string Name,
    string? Email,
    string? AvatarBase64);

namespace BlazorServer.Identity;

public sealed record CurrentUser(
    UserId UserId,
    OrganizationId OrganizationId,
    string DisplayName,
    string? Email = null,
    string? Avatar = null);
